namespace MiniOrders.Application.Customers;
public interface ICustomerQueryService
{
    Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken ct);
}
