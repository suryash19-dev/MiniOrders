using MiniOrders.Domain.Customers;

namespace MiniOrders.Application.Abstractions;
public interface ICustomerRepository
{
    void Add(Customer customer);
    Task<Customer?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Customer>> GetAllWithDetailsAsync(CancellationToken ct);
}
