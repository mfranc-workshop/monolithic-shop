using System.Data.Entity;

namespace GithubDashboard.Data
{
    class MainDatabaseContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}