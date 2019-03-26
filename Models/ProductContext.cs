using Microsoft.EntityFrameworkCore;
using vladandartem.Models;

namespace vladandartem.Models
{
    public class ProductContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}