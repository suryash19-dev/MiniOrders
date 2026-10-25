namespace MiniOrders.Application.Customers;
public interface ICustomerQueryService
{
    Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken ct);
    Task <IReadOnlyList<CustomerDto>> GetAllCustomersAsync(CancellationToken ct);
}
