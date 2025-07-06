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

        [Required, MaxLength(100)]
        public string Location { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public decimal Price { get; set; }
        [Required, EmailAddress]
        public string ContactEmail { get; set; }

        [Phone]
        public string? ContactPhone { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        public List<AdImage> Images { get; set; } = new();

        public bool IsPromoted { get; set; } = false;
        public DateTime? PromotionExpiresAt { get; set; }

    }
}
