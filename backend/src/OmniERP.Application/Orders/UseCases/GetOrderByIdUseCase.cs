using OmniERP.Application.Orders.Dtos;
using OmniERP.Application.Ports;

namespace OmniERP.Application.Orders.UseCases;

public sealed class GetOrderByIdUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse?> ExecuteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        if (orderId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(orderId), "El identificador del pedido debe ser mayor que cero.");
        }

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        return order is null ? null : OrderMapper.ToResponse(order);
    }
}
