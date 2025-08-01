﻿using System.ComponentModel.DataAnnotations;

namespace Anzoo.ViewModels.Ad
{
    public class AdDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        [Required, MaxLength(100)]
        public string Location { get; set; }
        public string ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ImageFileNames { get; set; }
        public decimal Price { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsPromoted { get; set; }
        public DateTime? PromotionExpiresAt { get; set; }
        public string UserId { get; set; }

    }

}
