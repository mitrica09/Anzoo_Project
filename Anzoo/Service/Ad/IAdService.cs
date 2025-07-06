using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Anzoo.Service.Ad
{
    public interface IAdService
    {
        Task<int?> Create(CreateAdViewModel adForm);
        Task<List<AdListViewModel>> GetAllAds();
        Task<AdDetailViewModel?> GetAdById(int id);
        Task<IEnumerable<SelectListItem>> GetCategoriesForDropdownMenu();
        Task<List<AdListViewModel>> GetMyAds();
        Task<UpdateAdViewModel?> GetAdForEditAsync(int id, string userId);
        Task<bool> UpdateAdAsync(UpdateAdViewModel model, string userId);
        Task<bool> DeleteAdAsync(int id, string userId);
        Task<AdListWithPaginationViewModel> GetAllAdsFilteredAsync(AdFilterViewModel filter);
        Task<bool> PromoteAd(int adId, string userId, int days);

    }
}
