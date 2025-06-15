using System.ComponentModel.DataAnnotations;

namespace Anzoo.ViewModels.Ad
{
    public class CreateAdViewModel
    {
        [Required, MinLength(10), MaxLength(70)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Display(Name = "Imagini")]
        public List<IFormFile>? Images { get; set; }
        [Required]
        [Range(0.01, 1000000)]
        public decimal Price { get; set; }

    }
}
