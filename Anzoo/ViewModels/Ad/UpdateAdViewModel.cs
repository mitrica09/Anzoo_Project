// ViewModels/Ad/UpdateAdViewModel.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Anzoo.ViewModels.Ad
{
    public class UpdateAdViewModel
    {
        public int Id { get; set; }

        [Required, MinLength(10), MaxLength(70)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        /* 🆕 */
        [Required, MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }

        public List<IFormFile>? NewImages { get; set; }
        public List<string>? ExistingImages { get; set; }
        public List<string>? ImagesToDelete { get; set; }
        public string? MainImage { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
