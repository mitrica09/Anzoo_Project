namespace Anzoo.ViewModels.Ad
{
    public class AdFilterViewModel
    {
        public int? CategoryId { get; set; }

        public string? Location { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string? SortBy { get; set; }
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 3; // cate anunturi sa apara pe pagina (paginare)
    }
}
