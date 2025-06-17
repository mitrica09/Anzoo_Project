using Anzoo.Data;
using Anzoo.Models;
using Anzoo.ViewModels.Ad;
using Microsoft.EntityFrameworkCore;

namespace Anzoo.Repository.Favorite
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _http;

        public FavoriteRepository(AppDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }


        public async Task AddToFavoritesAsync(string userId, int adId)
        {
            var existing = await _db.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.AdId == adId);

            if (existing != null)
                return;

            var favorite = new Models.Favorite
            {
                UserId = userId,
                AdId = adId,
                AddedAt = DateTime.UtcNow
            };

            _db.Favorites.Add(favorite);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AdListViewModel>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _db.Favorites
                .Include(f => f.Ad)
                    .ThenInclude(ad => ad.Images)
                .Include(f => f.Ad.Category)
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return favorites.Select(f => new AdListViewModel
            {
                Id = f.Ad.Id,
                Title = f.Ad.Title,
                Category = f.Ad.Category.Name,
                Location = f.Ad.Location,
                Price = f.Ad.Price,
                CreatedAt = f.Ad.CreatedAt,
                MainImage = f.Ad.Images.FirstOrDefault(i => i.IsMain)?.FileName
            }).ToList();
        }
        public async Task RemoveFromFavoritesAsync(string userId, int adId)
        {
            var favorite = await _db.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.AdId == adId);

            if (favorite != null)
            {
                _db.Favorites.Remove(favorite);
                await _db.SaveChangesAsync();
            }
        }
    }
}
