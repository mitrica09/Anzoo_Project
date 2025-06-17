using Anzoo.ViewModels.Ad;

namespace Anzoo.Service.Favorite
{
    public interface IFavoriteService
    {
        Task AddToFavoritesAsync(string userId, int adId);
        Task<List<AdListViewModel>> GetUserFavoritesAsync(string userId);
        Task RemoveFromFavoritesAsync(string userId, int adId);

    }
}
