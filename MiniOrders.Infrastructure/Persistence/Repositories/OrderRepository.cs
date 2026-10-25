using Microsoft.EntityFrameworkCore;
using MiniOrders.Application.Abstractions;
using MiniOrders.Domain.Orders;

namespace MiniOrders.Infrastructure.Persistence.Repositories;
public class OrderRepository(AppDbContext context) : IOrderRepository
{
    public void Add(Order order)
    {
        context.Orders.Add(order);
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        return await context.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct)
    {
        return await context.Orders
            .AsNoTracking()
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(ct);
    }
}
