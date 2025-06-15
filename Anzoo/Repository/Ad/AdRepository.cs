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

        public async Task<bool> Create(CreateAdViewModel adForm)
        {
            using var trx = await _db.Database.BeginTransactionAsync();
            try
            {
                var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // caută categoria în baza de date
                var category = await _db.Categories.FindAsync(adForm.CategoryId);
                if (category == null)
                    return false;

                var ad = new Models.Ad
                {
                    Title = adForm.Title,
                    Description = adForm.Description,
                    Location = adForm.Location,
                    Category = category,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Price = adForm.Price
                };

                _db.Ads.Add(ad);
                await _db.SaveChangesAsync();

                // salvează imaginile
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
                return true;
            }
            catch
            {
                await trx.RollbackAsync();
                return false;
            }
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

        public async Task<AdDetailViewModel?> GetAdById(int id)
        {
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
                    ImageFileNames = a.Images.Select(i => i.FileName).ToList()
                })
                .FirstOrDefaultAsync();
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

            // Obține categoriile din DB
            var categories = await _db.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == ad.CategoryId // ✅ marchează categoria actuală ca selectată
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
                ExistingImages = ad.Images.Select(i => i.FileName).ToList(),
                Categories = categories
            };
        }

        public async Task<bool> UpdateAsync(UpdateAdViewModel model, string userId)
        {
            var ad = await _db.Ads
                .Include(a => a.Images)
                .FirstOrDefaultAsync(a => a.Id == model.Id && a.UserId == userId);

            if (ad == null)
                return false;

            // 1. Actualizează câmpuri de bază
            ad.Title = model.Title;
            ad.Description = model.Description;
            ad.Location = model.Location;
            ad.Price = model.Price;
            ad.CategoryId = model.CategoryId;

            // 2. Șterge imaginile marcate pentru ștergere
            if (model.ImagesToDelete != null)
            {
                var toDelete = ad.Images
                    .Where(i => model.ImagesToDelete.Contains(i.FileName))
                    .ToList();

                foreach (var img in toDelete)
                {
                    var path = Path.Combine("wwwroot", "uploads", img.FileName);
                    if (File.Exists(path))
                        File.Delete(path);

                    ad.Images.Remove(img);
                }
            }

            // 3. Adaugă imagini noi
            if (model.NewImages != null && model.NewImages.Any())
            {
                var uploadPath = Path.Combine("wwwroot", "uploads");

                foreach (var img in model.NewImages)
                {
                    if (img.Length > 0)
                    {
                        var uniqueName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(uploadPath, uniqueName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }

                        ad.Images.Add(new AdImage
                        {
                            FileName = uniqueName,
                            IsMain = false
                        });
                    }
                }
            }

            // 4. Setează imaginea principală
            if (!string.IsNullOrEmpty(model.MainImage))
            {
                foreach (var img in ad.Images)
                {
                    img.IsMain = img.FileName == model.MainImage;
                }
            }

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

    }
}
