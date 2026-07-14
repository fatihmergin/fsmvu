using Microsoft.EntityFrameworkCore;
using OrderApi.Models;

namespace OrderApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();
        order.HasIndex(item => item.OrderNumber).IsUnique();
        order.Property(item => item.OrderNumber).HasMaxLength(40).IsRequired();
        order.Property(item => item.CustomerName).HasMaxLength(120).IsRequired();
        order.Property(item => item.ProductName).HasMaxLength(160).IsRequired();
        order.Property(item => item.UnitPrice).HasPrecision(18, 2);
        order.Property(item => item.DiscountRate).HasPrecision(5, 4);
        order.Property(item => item.TotalPrice).HasPrecision(18, 2);
    }
}
