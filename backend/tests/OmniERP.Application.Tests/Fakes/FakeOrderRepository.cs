using OmniERP.Application.Ports;
using OmniERP.Domain.Orders;

namespace OmniERP.Application.Tests.Fakes;

internal sealed class FakeOrderRepository : IOrderRepository
{
    private readonly Dictionary<int, Order> _orders = [];

    public int SaveCount { get; private set; }

    public void Add(Order order)
    {
        _orders[order.Id] = order;
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);

        return Task.FromResult(order);
    }

    public Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        SaveCount += 1;
        _orders[order.Id] = order;

        return Task.CompletedTask;
    }
}
