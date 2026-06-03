

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShainingOpt.Models;

namespace ShainingOpt.DataBase
{
    public class AppDbContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public DbSet<Category> Categories { get; set; }

     
        public DbSet<Size> Sizes { get; set; }

     
        public DbSet<Color> Colors { get; set; }

     
        public DbSet<Brand> Brands { get; set; }

 
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Role> Roles { get; set; }


        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductName)
                .UseCollation("NOCASE");

            modelBuilder.Entity<Brand>()
                .Property(b => b.BrandName)
                .UseCollation("NOCASE");
        }
    }
}
