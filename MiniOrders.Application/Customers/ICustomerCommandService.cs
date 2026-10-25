using MiniOrders.Application.Customers.Commands.CreateCustomer;

namespace MiniOrders.Application.Customers;
public interface ICustomerCommandService
{
    Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken ct);
}
