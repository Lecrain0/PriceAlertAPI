using Microsoft.EntityFrameworkCore;
using PriceAlertAPI.Models;

namespace PriceAlertAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<PriceAlert> PriceAlerts => Set<PriceAlert>();
    public DbSet<PriceHistory> PriceHistories => Set<PriceHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Alerts)
            .WithOne(a => a.Product)
            .HasForeignKey(a => a.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>()
            .Property(p => p.CurrentPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<PriceAlert>()
            .Property(a => a.TargetPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<PriceHistory>()
            .Property(h => h.Price)
            .HasColumnType("decimal(18,2)");
    }
}
