using MiniOrders.Domain.Orders;

namespace MiniOrders.Application.Abstractions;
public interface IOrderRepository
{
    void Add(Order order);
    Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct);
}
