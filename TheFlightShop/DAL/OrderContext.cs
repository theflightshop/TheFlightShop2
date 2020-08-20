using Microsoft.EntityFrameworkCore;
using TheFlightShop.DAL.Schemas;

namespace TheFlightShop.DAL
{
    public class OrderContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        private readonly string _connectionString;

        public OrderContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }
    }
}
