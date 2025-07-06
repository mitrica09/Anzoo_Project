using Anzoo.Data;
using Anzoo.Models;
using Anzoo.Repository.Ad;
using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Anzoo.Repository.Ad
{
    public class AdRepository : IAdRepository
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;

        public AdRepository(AppDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task<int?> Create(CreateAdViewModel adForm)
        {
            using var trx = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var category = await _db.Categories.FindAsync(adForm.CategoryId);
                if (category == null || user == null)
                    return null;

                var ad = new Models.Ad
                {
                    Title = adForm.Title,
                    Description = adForm.Description,
                    Location = adForm.Location,
                    Category = category,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Price = adForm.Price,
                    ContactEmail = user.Email,
                    ContactPhone = adForm.ContactPhone
                };

                _db.Ads.Add(ad);
                await _db.SaveChangesAsync();

                if (adForm.Images != null && adForm.Images.Count > 0)
                {
                    foreach (var file in adForm.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var path = Path.Combine("wwwroot/uploads", fileName);

                        Directory.CreateDirectory("wwwroot/uploads");
                        using var stream = new FileStream(path, FileMode.Create);
                        await file.CopyToAsync(stream);

                        ad.Images.Add(new AdImage
                        {
                            FileName = fileName,
                            IsMain = false
                        });
                    }
                    ad.Images.First().IsMain = true;
                }

                await _db.SaveChangesAsync();
                await trx.CommitAsync();

                return ad.Id; // ✅ returnăm ID-ul
            }
            catch
            {
                await trx.RollbackAsync();
                return null;
            }
        }


        public async Task<AdDetailViewModel?> GetAdById(int id)
        {
            var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    
            return await _db.Ads
                .Include(a => a.Images)
                .Include(a => a.Category)
                .Where(a => a.Id == id)
                .Select(a => new AdDetailViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Location = a.Location,
                    Category = a.Category.Name,
                    CreatedAt = a.CreatedAt,
                    Price = a.Price,
                    ContactEmail = a.ContactEmail,
                    ContactPhone = a.ContactPhone,
                    IsPromoted = a.IsPromoted, 
                    PromotionExpiresAt = a.PromotionExpiresAt,
                    UserId = a.UserId,
                    ImageFileNames = a.Images
                        .OrderBy(i => i.OrderIndex)
                        .Select(i => i.FileName)
                        .ToList(),
                    IsFavorite = userId != null && _db.Favorites.Any(f => f.UserId == userId && f.AdId == id)
                })
                .FirstOrDefaultAsync();
        }



        public async Task<List<AdListViewModel>> GetAllAds()
        {
            var ads = await _db.Ads
                .Include(a => a.Images)
                .Include(a => a.Category)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return ads.Select(a => new AdListViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Category = a.Category.Name,
                Location = a.Location,
                CreatedAt = a.CreatedAt,
                Price = a.Price,
                MainImage = a.Images.FirstOrDefault(i => i.IsMain)?.FileName
            }).ToList();
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesForDropdownMenu()
        {
            return await _db.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        public async Task<List<AdListViewModel>> GetMyAds(string userId)
        {
            var ads = await _db.Ads
                .Where(a => a.UserId == userId)
                .Include(a => a.Images)
                .Include(a => a.Category)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return ads.Select(a => new AdListViewModel
            {
                Id = a.Id,
                Title = a.Title,
                CreatedAt = a.CreatedAt,
                Location = a.Location,
                Price = a.Price,
                Category = a.Category.Name,
                MainImage = a.Images.FirstOrDefault(i => i.IsMain)?.FileName
            }).ToList();
        }

        public async Task<UpdateAdViewModel?> GetAdForEditAsync(int id, string userId)
        {
            var ad = await _db.Ads
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

            if (ad == null) return null;

            var categories = await _db.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == ad.CategoryId
                })
                .ToListAsync();

            return new UpdateAdViewModel
            {
                Id = ad.Id,
                Title = ad.Title,
                Description = ad.Description,
                Location = ad.Location,
                CategoryId = ad.CategoryId,
                Price = ad.Price,
                ContactPhone = ad.ContactPhone,
                ExistingImages = ad.Images.Select(i => i.FileName).ToList(),
                Categories = categories
            };
        }

        public async Task<bool> UpdateAsync(UpdateAdViewModel model, string userId)
        {
            var ad = await _db.Ads
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.UserId == userId);

            if (ad == null) return false;

            ad.Title = model.Title;
            ad.Description = model.Description;
            ad.Location = model.Location;
            ad.Price = model.Price;
            ad.CategoryId = model.CategoryId;
            ad.ContactPhone = model.ContactPhone;

            /* 1. Ștergere imagini selectate */
            if (model.ImagesToDelete?.Any() == true)
            {
                foreach (var img in ad.Images
                                      .Where(i => model.ImagesToDelete.Contains(i.FileName))
                                      .ToList())
                {
                    var path = Path.Combine("wwwroot", "uploads", img.FileName);
                    if (File.Exists(path)) File.Delete(path);
                    ad.Images.Remove(img);
                }
            }

            /* 2. Adăugare imagini noi */
            if (model.NewImages?.Any() == true)
            {
                var uploadDir = Path.Combine("wwwroot", "uploads");
                Directory.CreateDirectory(uploadDir);

                foreach (var file in model.NewImages.Where(f => f.Length > 0))
                {
                    var unique = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var dest = Path.Combine(uploadDir, unique);

                    await using var fs = new FileStream(dest, FileMode.Create);
                    await file.CopyToAsync(fs);

                    ad.Images.Add(new AdImage
                    {
                        FileName = unique,
                        IsMain = false,
                        OrderIndex = 9999 // temporar
                    });
                }
            }

            /* 3. Actualizare ordine și poză principală */
            var finalOrder = new List<string>();

            if (model.ExistingImages?.Any() == true)
                finalOrder.AddRange(model.ExistingImages);

            if (model.NewImages?.Any() == true)
            {
                var newFileNames = ad.Images
                    .Where(x => x.OrderIndex == 9999)
                    .Select(x => x.FileName)
                    .ToList();

                finalOrder.AddRange(newFileNames);
            }

            for (int i = 0; i < finalOrder.Count; i++)
            {
                var img = ad.Images.FirstOrDefault(x => x.FileName == finalOrder[i]);
                if (img != null)
                {
                    img.OrderIndex = i;
                    img.IsMain = img.FileName == model.MainImage;
                }
            }

            if (ad.Images.Any() && ad.Images.All(x => !x.IsMain))
                ad.Images.OrderBy(x => x.OrderIndex).First().IsMain = true;

            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var ad = await _db.Ads.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
            if (ad == null) return false;

            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<AdListWithPaginationViewModel> GetAllAdsFilteredAsync(AdFilterViewModel filter)
        {
            var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // ✅ Dezactivează promovările expirate
            var now = DateTime.Now;
            var expiredAds = await _db.Ads
                .Where(a => a.IsPromoted && a.PromotionExpiresAt < now)
                .ToListAsync();

            foreach (var ad in expiredAds)
            {
                ad.IsPromoted = false;
                ad.PromotionExpiresAt = null;
            }

            if (expiredAds.Any())
                await _db.SaveChangesAsync();

            // 🔎 Aplică filtrarea normală
            var query = _db.Ads
                .Include(a => a.Images)
                .Include(a => a.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim().ToLower();
                query = query.Where(a =>
                    a.Title.ToLower().Contains(keyword) ||
                    (a.Description != null && a.Description.ToLower().Contains(keyword)));
            }

            if (filter.CategoryId.HasValue && filter.CategoryId.Value > 0)
                query = query.Where(a => a.CategoryId == filter.CategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                var location = filter.Location.Trim().ToLower();
                query = query.Where(a => a.Location.ToLower().Contains(location));
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(a => a.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(a => a.Price <= filter.MaxPrice.Value);

            query = filter.SortBy switch
            {
                "price_asc" => query
                    .OrderByDescending(a => a.IsPromoted)
                    .ThenBy(a => a.Price),

                "price_desc" => query
                    .OrderByDescending(a => a.IsPromoted)
                    .ThenByDescending(a => a.Price),

                "date_asc" => query
                    .OrderByDescending(a => a.IsPromoted)
                    .ThenBy(a => a.CreatedAt),

                _ => query
                    .OrderByDescending(a => a.IsPromoted)
                    .ThenByDescending(a => a.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var ads = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var favoriteAdIds = new HashSet<int>();
            if (!string.IsNullOrEmpty(userId))
            {
                favoriteAdIds = _db.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.AdId)
                    .ToHashSet();
            }

            return new AdListWithPaginationViewModel
            {
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount,
                Ads = ads.Select(a => new AdListViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Category = a.Category.Name,
                    Location = a.Location,
                    CreatedAt = a.CreatedAt,
                    Price = a.Price,
                    MainImage = a.Images.FirstOrDefault(i => i.IsMain)?.FileName,
                    IsFavorite = favoriteAdIds.Contains(a.Id),
                    IsPromoted = a.IsPromoted
                }).ToList()
            };
        }

        public async Task<bool> PromoteAd(int adId, string userId, int days)
        {
            var ad = await _db.Ads.FirstOrDefaultAsync(a => a.Id == adId && a.UserId == userId);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (ad == null || user == null || user.Points < days)
                return false;

            ad.IsPromoted = true;
            ad.PromotionExpiresAt = DateTime.Now.AddDays(days);
            user.Points -= days;

            await _db.SaveChangesAsync();
            return true;
        }

    }
}
