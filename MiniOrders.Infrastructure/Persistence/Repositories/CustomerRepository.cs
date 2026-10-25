using Microsoft.EntityFrameworkCore;
using MiniOrders.Application.Abstractions;
using MiniOrders.Domain.Customers;

namespace MiniOrders.Infrastructure.Persistence.Repositories;
public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public void Add(Customer customer)
    {
        context.Customers.Add(customer);
    }

    public Task<Customer?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct)
    {
        return context.Customers
            .AsNoTracking()
            .Include(c => c.Profile)
            .Include(c => c.Orders)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        return context.Customers.AnyAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Customer>> GetAllWithDetailsAsync(CancellationToken ct)
    {
        return await context.Customers
            .AsNoTracking()
            .Include(c => c.Profile)
            .Include(c => c.Orders.OrderByDescending(o => o.OrderDate))
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }
}
