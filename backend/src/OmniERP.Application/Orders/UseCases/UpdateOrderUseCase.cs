using OmniERP.Application.Common;
using OmniERP.Application.Orders.Dtos;
using OmniERP.Application.Ports;
using OmniERP.Domain.Orders;

namespace OmniERP.Application.Orders.UseCases;

public sealed class UpdateOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public UpdateOrderUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<UpdateOrderResult> ExecuteAsync(
        int orderId,
        UpdateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (orderId <= 0)
        {
            return UpdateOrderResult.ValidationError("El identificador del pedido debe ser mayor que cero.");
        }

        if (request is null)
        {
            return UpdateOrderResult.ValidationError("La solicitud de actualizacion del pedido es requerida.");
        }

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
        {
            return UpdateOrderResult.NotFound(orderId);
        }

        try
        {
            order.Update(
                request.CustomerName,
                request.CustomerEmail,
                request.DeliveryAddress,
                request.InternalComment,
                request.StatusId,
                request.ShippingMethodId,
                request.Version,
                request.UpdatedBy);
        }
        catch (OrderConflictException)
        {
            var conflict = new OrderConflictResponse
            {
                Code = ErrorCodes.OrderConcurrencyConflict,
                Message = "El pedido fue actualizado por otro usuario. Revisa la informacion actual antes de guardar nuevamente.",
                CurrentOrder = OrderMapper.ToResponse(order),
                AttemptedChanges = request
            };

            return UpdateOrderResult.ConflictResult(conflict);
        }
        catch (ArgumentException exception)
        {
            return UpdateOrderResult.ValidationError(exception.Message);
        }

        await _orderRepository.SaveAsync(order, cancellationToken);

        return UpdateOrderResult.Success(OrderMapper.ToResponse(order));
    }
}
