using OmniERP.Application.Orders.Dtos;

namespace OmniERP.Application.Orders.UseCases;

public sealed record UpdateOrderResult
{
    private UpdateOrderResult(
        UpdateOrderResultStatus status,
        string message,
        OrderResponse? order = null,
        OrderConflictResponse? conflict = null)
    {
        Status = status;
        Message = message;
        Order = order;
        Conflict = conflict;
    }

    public UpdateOrderResultStatus Status { get; }

    public string Message { get; }

    public OrderResponse? Order { get; }

    public OrderConflictResponse? Conflict { get; }

    public bool IsSuccess => Status == UpdateOrderResultStatus.Success;

    public static UpdateOrderResult Success(OrderResponse order)
    {
        return new UpdateOrderResult(
            UpdateOrderResultStatus.Success,
            "Pedido actualizado correctamente.",
            order);
    }

    public static UpdateOrderResult NotFound(int orderId)
    {
        return new UpdateOrderResult(
            UpdateOrderResultStatus.NotFound,
            $"No se encontro el pedido {orderId}.");
    }

    public static UpdateOrderResult ConflictResult(OrderConflictResponse conflict)
    {
        return new UpdateOrderResult(
            UpdateOrderResultStatus.Conflict,
            conflict.Message,
            conflict: conflict);
    }

    public static UpdateOrderResult ValidationError(string message)
    {
        return new UpdateOrderResult(UpdateOrderResultStatus.ValidationError, message);
    }
}

public enum UpdateOrderResultStatus
{
    Success,
    NotFound,
    Conflict,
    ValidationError
}
