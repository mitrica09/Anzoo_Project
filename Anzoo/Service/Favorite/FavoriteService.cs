using Anzoo.Repository.Favorite;
using Anzoo.ViewModels.Ad;

namespace Anzoo.Service.Favorite
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task AddToFavoritesAsync(string userId, int adId)
        {
             await _favoriteRepository.AddToFavoritesAsync(userId, adId);
        }

        public async Task<List<AdListViewModel>> GetUserFavoritesAsync(string userId)
        {
            return await _favoriteRepository.GetUserFavoritesAsync(userId);
        }
        public async Task RemoveFromFavoritesAsync(string userId, int adId)
        {
            await _favoriteRepository.RemoveFromFavoritesAsync(userId, adId);
        }
    }
}
