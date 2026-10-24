using MiniOrders.Application.Orders.Commands.CreateOrder;

namespace MiniOrders.Application.Orders;
public interface IOrderCommandService
{
    Task<Guid> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken = default);
}
