namespace OmniERP.Domain.Orders;

public sealed class OrderConflictException : Exception
{
    public OrderConflictException(int orderId, int currentVersion, int attemptedVersion)
        : base($"El pedido {orderId} fue modificado por otro usuario. Version actual: {currentVersion}, version enviada: {attemptedVersion}.")
    {
        OrderId = orderId;
        CurrentVersion = currentVersion;
        AttemptedVersion = attemptedVersion;
    }

    public int OrderId { get; }

    public int CurrentVersion { get; }

    public int AttemptedVersion { get; }
}
