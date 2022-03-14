using Microsoft.EntityFrameworkCore;

namespace CryptowatcherA.Model
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Currency> Currency { get; set; }
    }
}
