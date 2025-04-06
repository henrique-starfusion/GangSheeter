using GangSheeter.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace GangSheeter.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<PrintSheet> PrintSheets { get; set; }
        public DbSet<ImageModel> Images { get; set; }
        public DbSet<AppSettings> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GangSheeter", "data.db");
            var directoryPath = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            var connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrintSheet>()
                .HasMany(p => p.Images)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppSettings>()
                .HasData(new AppSettings
                {
                    Id = 1,
                    SheetWidthCm = PrintSheet.DefaultWidthCm,
                    MinSheetHeightCm = 10,
                    MaxSheetHeightCm = PrintSheet.MaxHeightCm,
                    ExportDpi = 300,
                    CompressionType = "LZW"
                });
        }
    }

    public class AppSettings
    {
        public int Id { get; set; }
        public float SheetWidthCm { get; set; }
        public float MinSheetHeightCm { get; set; }
        public float MaxSheetHeightCm { get; set; }
        public int ExportDpi { get; set; }
        public string? CompressionType { get; set; }
    }
}
