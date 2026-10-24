using AutoMapper;
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
    private readonly IMapper _mapper;
    public CustomerCommandService(
        IUnitOfWork uow, IValidator<CreateCustomerCommand> validator,
        ILogger<CustomerCommandService> logger, IMapper mapper)
    {
        _uow = uow;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Guid> CreateCustomerAsync(CreateCustomerCommand command, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(command, ct);
        
        var customer = _mapper.Map<Customer>(command);

        _uow.Customers.Add(customer);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Customer created with ID: {CustomerId}", customer.Id);
        return customer.Id;
    }
}
