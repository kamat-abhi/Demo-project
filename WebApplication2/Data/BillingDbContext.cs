// Put in /Data/BillingDbContext.cs
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class BillingDbContext : DbContext
    {
        public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Depot> Depots { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillItemEntity> BillItems { get; set; }
        public DbSet<BillTerm> BillTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Bill>().HasIndex(x => x.BillNo).IsUnique();
            foreach (var p in b.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                p.SetColumnType("decimal(18,2)");
        }
    }
}
