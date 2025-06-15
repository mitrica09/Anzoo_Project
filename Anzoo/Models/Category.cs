// Models/Category.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Anzoo.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        // Relație inversă (opțională, dar utilă)
        public ICollection<Ad> Ads { get; set; }
    }
}
