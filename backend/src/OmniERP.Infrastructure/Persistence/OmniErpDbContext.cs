using Microsoft.EntityFrameworkCore;
using OmniERP.Domain.Orders;

namespace OmniERP.Infrastructure.Persistence;

public sealed class OmniErpDbContext : DbContext
{
    public OmniErpDbContext(DbContextOptions<OmniErpDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureOrders(modelBuilder);
        ConfigureOrderItems(modelBuilder);
    }

    private static void ConfigureOrders(ModelBuilder modelBuilder)
    {
        var order = modelBuilder.Entity<Order>();

        order.HasKey(item => item.Id);

        order.Property(item => item.CustomerName)
            .IsRequired();

        order.Property(item => item.CustomerEmail)
            .IsRequired();

        order.Property(item => item.DeliveryAddress)
            .IsRequired();

        order.Property(item => item.InternalComment)
            .IsRequired();

        order.Property(item => item.UpdatedBy)
            .IsRequired();

        order.HasMany(item => item.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        order.Navigation(item => item.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }

    private static void ConfigureOrderItems(ModelBuilder modelBuilder)
    {
        var orderItem = modelBuilder.Entity<OrderItem>();

        orderItem.HasKey(item => item.Id);

        orderItem.Property(item => item.OrderId);

        orderItem.Property(item => item.ProductSku)
            .IsRequired();

        orderItem.Property(item => item.ProductName)
            .IsRequired();

        orderItem.Property(item => item.Quantity);

        orderItem.Property(item => item.UnitPrice)
            .HasPrecision(18, 2);
    }
}
