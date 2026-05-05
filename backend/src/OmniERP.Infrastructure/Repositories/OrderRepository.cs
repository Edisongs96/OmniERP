using Microsoft.EntityFrameworkCore;
using OmniERP.Application.Ports;
using OmniERP.Domain.Orders;
using OmniERP.Infrastructure.Persistence;

namespace OmniERP.Infrastructure.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OmniErpDbContext _dbContext;

    public OrderRepository(OmniErpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Orders
            .Include(order => order.Items)
            .FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.Orders
            .AnyAsync(existing => existing.Id == order.Id, cancellationToken);

        if (!exists)
        {
            await _dbContext.Orders.AddAsync(order, cancellationToken);
        }
        else if (_dbContext.Entry(order).State == EntityState.Detached)
        {
            _dbContext.Orders.Update(order);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
