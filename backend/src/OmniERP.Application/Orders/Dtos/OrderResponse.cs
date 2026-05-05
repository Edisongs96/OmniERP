namespace OmniERP.Application.Orders.Dtos;

public sealed record OrderResponse
{
    public int Id { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public string CustomerEmail { get; init; } = string.Empty;

    public string DeliveryAddress { get; init; } = string.Empty;

    public string InternalComment { get; init; } = string.Empty;

    public int StatusId { get; init; }

    public int ShippingMethodId { get; init; }

    public int Version { get; init; }

    public DateTime UpdatedAt { get; init; }

    public string UpdatedBy { get; init; } = string.Empty;

    public IReadOnlyCollection<OrderItemResponse> Items { get; init; } = [];
}
