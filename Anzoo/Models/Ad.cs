using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;

namespace Anzoo.Models
{
    public class Ad
    {
        public int Id { get; set; }

        [Required, MinLength(10), MaxLength(70)]
        public string Title { get; set; }
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string UserId { get; set; }
        public User User { get; set; }
        public List<AdImage> Images { get; set; } = new();
        public decimal Price { get; set; }
    }
}
