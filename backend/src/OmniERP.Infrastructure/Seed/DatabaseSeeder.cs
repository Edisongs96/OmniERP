using Microsoft.EntityFrameworkCore;
using OmniERP.Domain.Orders;
using OmniERP.Infrastructure.Persistence;

namespace OmniERP.Infrastructure.Seed;

public sealed class DatabaseSeeder
{
    private readonly OmniErpDbContext _dbContext;

    public DatabaseSeeder(OmniErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var orderExists = await _dbContext.Orders
            .AnyAsync(order => order.Id == 1001, cancellationToken);

        if (orderExists)
        {
            return;
        }

        var order = new Order(
            id: 1001,
            customerName: "Cliente Demo",
            customerEmail: "cliente.demo@omnierp.local",
            deliveryAddress: "Calle 10 # 20-30, Medellin",
            internalComment: "Pedido prioritario",
            statusId: 1,
            shippingMethodId: 2,
            version: 1,
            updatedAt: DateTime.UtcNow,
            updatedBy: "system",
            items:
            [
                new OrderItem(
                    id: 1,
                    orderId: 1001,
                    productSku: "SKU-001",
                    productName: "Producto Demo",
                    quantity: 2,
                    unitPrice: 50000m)
            ]);

        await _dbContext.Orders.AddAsync(order, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
