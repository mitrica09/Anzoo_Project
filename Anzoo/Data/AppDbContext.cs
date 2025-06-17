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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Favorite> Favorites { get; set; }



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

            // Ad ↔ Category
            modelBuilder.Entity<Ad>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Ads)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);   // nu șterge anunțurile la ștergerea categoriei

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Auto, moto și ambarcațiuni" },
                new Category { Id = 2, Name = "Imobiliare" },
                new Category { Id = 3, Name = "Locuri de muncă" },
                new Category { Id = 4, Name = "Electronice și electrocasnice" },
                new Category { Id = 5, Name = "Modă și frumusețe" },
                new Category { Id = 6, Name = "Piese auto" },
                new Category { Id = 7, Name = "Casă și grădină" },
                new Category { Id = 8, Name = "Mama și copilul" },
                new Category { Id = 9, Name = "Sport, timp liber, artă" },
                new Category { Id = 10, Name = "Animale de companie" },
                new Category { Id = 11, Name = "Agro și industrie" },
                new Category { Id = 12, Name = "Servicii" },
                new Category { Id = 13, Name = "Echipamente profesionale" }
                );

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Ad)
                .WithMany()
                .HasForeignKey(f => f.AdId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}

