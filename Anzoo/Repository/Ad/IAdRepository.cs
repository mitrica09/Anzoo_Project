using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Anzoo.Repository.Ad
{
    public interface IAdRepository
    {
        Task<int?> Create(CreateAdViewModel adForm);
        Task<List<AdListViewModel>> GetAllAds();
        Task<AdDetailViewModel?> GetAdById(int id);
        Task<IEnumerable<SelectListItem>> GetCategoriesForDropdownMenu();
        Task<List<AdListViewModel>> GetMyAds(string userId);
        Task<UpdateAdViewModel?> GetAdForEditAsync(int id, string userId);
        Task<bool> UpdateAsync(UpdateAdViewModel model, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<AdListWithPaginationViewModel> GetAllAdsFilteredAsync(AdFilterViewModel filter);
        Task<bool> PromoteAd(int adId, string userId, int days);
    }
}
