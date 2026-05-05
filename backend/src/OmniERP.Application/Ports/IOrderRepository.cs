using OmniERP.Domain.Orders;

namespace OmniERP.Application.Ports;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task SaveAsync(Order order, CancellationToken cancellationToken = default);
}
