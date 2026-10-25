namespace MiniOrders.Application.Orders;
public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken ct);
    Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct);
}
