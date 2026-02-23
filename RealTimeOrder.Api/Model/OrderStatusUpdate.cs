namespace RealTimeOrder.Api.Model;

public enum OrderStatus
{
    Created,
    PaymentConfirmed,
    Packed,
    Shipped,
    OutForDelivery,
    Delivered
}

public sealed record OrderStatusUpdate(
    Guid OrderId,
    OrderStatus Status,
    DateTime Timestamp);