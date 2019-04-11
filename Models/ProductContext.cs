using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using vladandartem.Models;
using vladandartem.ClassHelpers;

namespace vladandartem.Models
{
    public class ProductContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartProduct> CartProduct { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Cart> Cart { get; set; }

        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}