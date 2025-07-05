using Anzoo.Repository.Ad;
using Anzoo.ViewModels.Ad;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.WebRequestMethods;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Anzoo.Service.Ad
{
    public class AdService : IAdService
    {
        private readonly IHttpContextAccessor _http;
        private readonly IAdRepository _adRepository;

        public AdService(IAdRepository adRepository, IHttpContextAccessor http)
        {
            _adRepository = adRepository;
            _http = http;
        }

        public async Task<bool> Create(CreateAdViewModel adForm)
        {
            return await _adRepository.Create(adForm);
        }
        public async Task<List<AdListViewModel>> GetAllAds()
        {
            return await _adRepository.GetAllAds();
        }
        public async Task<AdDetailViewModel?> GetAdById(int id)
        {
            return await _adRepository.GetAdById(id);
        }

        public async Task<IEnumerable<SelectListItem>> GetCategoriesForDropdownMenu()
        {
            return await _adRepository.GetCategoriesForDropdownMenu();
        }
        public async Task<List<AdListViewModel>> GetMyAds()
        {
            var userId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _adRepository.GetMyAds(userId!);
        }
        public async Task<bool> UpdateAdAsync(UpdateAdViewModel model, string userId)
        {
            return await _adRepository.UpdateAsync(model, userId);
        }

        public async Task<bool> DeleteAdAsync(int id, string userId)
        {
            return await _adRepository.DeleteAsync(id, userId);
        }

        public Task<UpdateAdViewModel?> GetAdForEditAsync(int id, string userId)
        {
            return _adRepository.GetAdForEditAsync(id, userId);
        }
        public async Task<AdListWithPaginationViewModel> GetAllAdsFilteredAsync(AdFilterViewModel filter)
        {
            return await _adRepository.GetAllAdsFilteredAsync(filter);
        }


    }
}
