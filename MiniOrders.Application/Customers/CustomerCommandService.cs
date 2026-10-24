using FluentValidation;
using Microsoft.Extensions.Logging;
using MiniOrders.Application.Abstractions;
using MiniOrders.Application.Customers.Commands.CreateCustomer;
using MiniOrders.Domain.Customers;

namespace MiniOrders.Application.Customers;
public class CustomerCommandService : ICustomerCommandService
{
    private readonly IUnitOfWork _uow;
    private readonly IValidator<CreateCustomerCommand> _validator;
    private readonly ILogger<CustomerCommandService> _logger;
    public CustomerCommandService(IUnitOfWork uow, IValidator<CreateCustomerCommand> validator, ILogger<CustomerCommandService> logger)
    {
        _uow = uow;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var customerId = Guid.NewGuid();
        var customer = new Customer
        {

        }
    }
}
