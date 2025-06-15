using Anzoo.Repository.Ad;
using Anzoo.ViewModels.Ad;

namespace Anzoo.Service.Ad
{
    public class AdService : IAdService
    {
        private readonly IAdRepository _adRepository;

        public AdService(IAdRepository adRepository)
        {
            _adRepository = adRepository;
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

    }
}
