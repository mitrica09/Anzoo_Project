using Anzoo.Data;
using Anzoo.Models;
using Anzoo.Repository.Ad;
using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Http;
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

                var ad = new Models.Ad
                {
                    Title = adForm.Title,
                    Description = adForm.Description,
                    Category = adForm.Category,
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    Price = adForm.Price
                };

                // salvează anunțul pentru a genera Id
                _db.Ads.Add(ad);
                await _db.SaveChangesAsync();

                // salvează imaginile, dacă există
                if (adForm.Images != null && adForm.Images.Count > 0)
                {
                    foreach (var file in adForm.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var path = Path.Combine("wwwroot/uploads", fileName);

                        Directory.CreateDirectory("wwwroot/uploads");
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        ad.Images.Add(new AdImage
                        {
                            FileName = fileName,
                            IsMain = false
                        });
                    }
                    // setează prima imagine ca principală
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
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync(); // forțează executarea în memorie

            return ads.Select(a => new AdListViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Category = a.Category,
                CreatedAt = a.CreatedAt,
                Price = a.Price,
                MainImage = a.Images.FirstOrDefault(i => i.IsMain)?.FileName
            }).ToList();
        }

        public async Task<AdDetailViewModel?> GetAdById(int id)
        {
            return await _db.Ads
                .Where(a => a.Id == id)
                .Select(a => new AdDetailViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    Category = a.Category,
                    CreatedAt = a.CreatedAt,
                    Price = a.Price,
                    ImageFileNames = a.Images.Select(i => i.FileName).ToList()
                })
                .FirstOrDefaultAsync();
        }



    }
}
