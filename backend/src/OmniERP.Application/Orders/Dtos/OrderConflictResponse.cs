using OmniERP.Application.Common;

namespace OmniERP.Application.Orders.Dtos;

public sealed record OrderConflictResponse
{
    public string Code { get; init; } = ErrorCodes.OrderConcurrencyConflict;

    public string Message { get; init; } = string.Empty;

    public required OrderResponse CurrentOrder { get; init; }

    public required UpdateOrderRequest AttemptedChanges { get; init; }
}
