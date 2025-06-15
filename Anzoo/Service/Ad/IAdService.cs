using Anzoo.ViewModels.Ad;

namespace Anzoo.Service.Ad
{
    public interface IAdService
    {
        Task<bool> Create(CreateAdViewModel adForm);
        Task<List<AdListViewModel>> GetAllAds();
        Task<AdDetailViewModel?> GetAdById(int id);

    }
}
