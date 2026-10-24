using MiniOrders.Application.Abstractions;
using MiniOrders.Infrastructure.Persistence.Repositories;

namespace MiniOrders.Infrastructure.Persistence;
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private ICustomerRepository? _customers;
    private IOrderRepository? _orders;

    public ICustomerRepository Customers => _customers ??= new CustomerRepository(context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(context);

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => context.SaveChangesAsync(ct);
}
