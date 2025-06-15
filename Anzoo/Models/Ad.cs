using System;
using System.Collections.Generic;
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

        // 🆕 Locația anunțului (de unde se vinde)
        [Required, MaxLength(100)]
        public string Location { get; set; }

        // Relație cu categoria
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public decimal Price { get; set; }

        // Relație cu utilizatorul
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        // Imagini atașate
        public List<AdImage> Images { get; set; } = new();
    }
}
