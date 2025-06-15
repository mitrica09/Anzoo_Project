using Anzoo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Anzoo.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<AdImage> AdImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ad ↔ AdImage
            modelBuilder.Entity<Ad>()
                .HasMany(a => a.Images)
                .WithOne(i => i.Ad)
                .HasForeignKey(i => i.AdId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ↔ Ads
            modelBuilder.Entity<Ad>()
                .HasOne(a => a.User)
                .WithMany() // poți pune .WithMany(u => u.Ads) dacă ai adăugat lista în User
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // sau Cascade, în funcție de logică

            modelBuilder.Entity<Ad>()
                .Property(a => a.Price)
                .HasPrecision(10, 2); // 10 cifre în total, 2 după virgulă
        }


    }
}

