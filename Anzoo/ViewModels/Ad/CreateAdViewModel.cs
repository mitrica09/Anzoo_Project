// ViewModels/Ad/CreateAdViewModel.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Anzoo.ViewModels.Ad
{
    public class CreateAdViewModel
    {
        [Required, MinLength(10), MaxLength(70)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Locația este obligatorie"), MaxLength(100)]
        public string Location { get; set; }

        [Phone]
        public string? ContactPhone { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public decimal Price { get; set; }

        public List<IFormFile>? Images { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
