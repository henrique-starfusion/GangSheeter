using GangSheeter.Models;
using Microsoft.EntityFrameworkCore;

namespace GangSheeter.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ImageInfo> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=gangsheeter.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ImageInfo>()
                .HasKey(i => i.Id);
        }
    }
}
