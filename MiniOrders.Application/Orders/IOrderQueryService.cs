namespace MiniOrders.Application.Orders;
public interface IOrderQueryService
{
    Task<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
