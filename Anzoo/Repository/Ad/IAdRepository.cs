using Anzoo.ViewModels.Ad;

namespace Anzoo.Repository.Ad
{
    public interface IAdRepository
    {
        Task<bool> Create(CreateAdViewModel adForm);
        Task<List<AdListViewModel>> GetAllAds();
        Task<AdDetailViewModel?> GetAdById(int id);

    }
}
