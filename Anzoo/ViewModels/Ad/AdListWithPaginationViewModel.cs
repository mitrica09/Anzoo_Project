namespace Anzoo.ViewModels.Ad
{
    public class AdListWithPaginationViewModel
    {
        public List<AdListViewModel> Ads { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalCount / PageSize);
    }
}
