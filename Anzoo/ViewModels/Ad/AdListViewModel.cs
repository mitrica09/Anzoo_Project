using System.ComponentModel.DataAnnotations;

namespace Anzoo.ViewModels.Ad
{
    public class AdListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        [Required, MaxLength(100)]
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? MainImage { get; set; }
        public decimal Price { get; set; }

    }
}
