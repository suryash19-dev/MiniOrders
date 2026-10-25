using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MiniOrders.Application.Abstractions;
using MiniOrders.Application.Exceptions;
using MiniOrders.Application.Orders.Commands.CreateOrder;
using MiniOrders.Domain.Customers;
using MiniOrders.Domain.Orders;

namespace MiniOrders.Application.Orders;
public class OrderCommandService : IOrderCommandService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderCommand> _validator;
    private readonly ILogger<OrderCommandService> _logger;

    public OrderCommandService(IUnitOfWork uow, IMapper mapper, IValidator<CreateOrderCommand> validator, ILogger<OrderCommandService> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> CreateOrderAsync(CreateOrderCommand command, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        if(!await _uow.Customers.ExistsAsync(command.CustomerId, ct))
        {
            throw new NotFoundException(nameof(Customer), command.CustomerId);
        }

        var order = _mapper.Map<Order>(command);
        _uow.Orders.Add(order);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Order created with ID: {OrderId} for customerId: {CustomerId} ", order.Id, order.CustomerId);
        return order.Id;
    }
}
