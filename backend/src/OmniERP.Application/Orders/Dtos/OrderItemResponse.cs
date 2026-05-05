namespace OmniERP.Application.Orders.Dtos;

public sealed record OrderItemResponse
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string ProductSku { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
}
