using AutoMapper;
using MiniOrders.Application.Abstractions;
using MiniOrders.Application.Exceptions;
using MiniOrders.Domain.Customers;

namespace MiniOrders.Application.Orders;
public class OrderQueryService : IOrderQueryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public OrderQueryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        if(!await _uow.Customers.ExistsAsync(customerId, ct))
        {
            throw new NotFoundException(nameof(Customer), customerId);
        }

        var orders = await _uow.Orders.GetByCustomerIdAsync(customerId, ct);
        return _mapper.Map<IReadOnlyList<OrderDto>>(orders);
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllOrdersAsync(CancellationToken ct)
    {
        var orders = await _uow.Orders.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<OrderDto>>(orders);
    }
}
