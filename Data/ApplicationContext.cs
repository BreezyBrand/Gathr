using Microsoft.EntityFrameworkCore;
using CrummyApp.Models;

namespace CrummyApp.Data
{
    public class ApplicationContext : DbContext
    {

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Card> Cards { get; set; }
        public DbSet<InvOptions> Inventory { get; set; }
        public DbSet<PriceOptions> Pricing { get; set; }
        
    }
}
