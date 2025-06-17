using Anzoo.Models;
using Anzoo.ViewModels.Ad;


namespace Anzoo.Repository.Favorite
{
    public interface IFavoriteRepository
    {
        Task AddToFavoritesAsync(string userId, int adId);
        Task<List<AdListViewModel>> GetUserFavoritesAsync(string userId);
        Task RemoveFromFavoritesAsync(string userId, int adId);
    }
}
