using Microsoft.EntityFrameworkCore;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.DAL
{
    public class ProductContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<MaintenanceItem> MaintenanceItems { get; set; }

        private readonly string _connectionString;

        public ProductContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }
    }
}
