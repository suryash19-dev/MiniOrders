namespace MiniOrders.Application.Orders.Commands.CreateOrder;
public sealed record CreateOrderCommand(Guid CustomerId, DateTime OrderDate, decimal TotalAmount);