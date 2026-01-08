using Microsoft.EntityFrameworkCore;
using Gathr.Models;

namespace Gathr.Data
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
        public DbSet<CardImages> Images { get; set; }
        public DbSet<CardLocation> Locations { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<InvTag> InvTags { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }
        public DbSet<SpreadsheetRow> SpreadsheetRows { get; set; }
    }
}
