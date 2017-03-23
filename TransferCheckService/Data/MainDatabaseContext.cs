using System.Data.Entity;

namespace TransferCheckService.Data
{
    class MainDatabaseContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Buyer> Buyers { get; set; }
    }
}