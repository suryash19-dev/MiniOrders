using AutoMapper;
using MiniOrders.Application.Abstractions;
using MiniOrders.Application.Exceptions;
using MiniOrders.Domain.Customers;

namespace MiniOrders.Application.Customers;
public class CustomerQueryService : ICustomerQueryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CustomerQueryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<CustomerDto> GetCustomerByIdAsync(Guid id, CancellationToken ct)
    {
        var customer = await _uow.Customers.GetByIdWithDetailsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Customer), id);

        return _mapper.Map<CustomerDto>(customer);
    }
}
