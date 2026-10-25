using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MiniOrders.Application.Customers;
using MiniOrders.Application.Orders;
using MiniOrders.Application.Profiles;
using MiniOrders.Infrastructure.Persistence;

namespace MiniOrders.Tests.TestSupport;

public sealed class TestDatabase : IDisposable
{
    private readonly DbContextOptions<AppDbContext> _options;

    public TestDatabase()
    {
        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        Context = new AppDbContext(_options);
        UnitOfWork = new UnitOfWork(Context);
        Mapper = new MapperConfiguration(
            configuration => configuration.AddMaps(typeof(CustomerMappingProfile).Assembly)).CreateMapper();
    }

    public AppDbContext Context { get; }
    public UnitOfWork UnitOfWork { get; }
    public IMapper Mapper { get; }

    public CustomerCommandService CreateCustomerCommandService() => new(
        UnitOfWork,
        new MiniOrders.Application.Customers.Commands.CreateCustomer.CreateCustomerCommandValidator(),
        NullLogger<CustomerCommandService>.Instance,
        Mapper);

    public CustomerQueryService CreateCustomerQueryService() => new(UnitOfWork, Mapper);

    public OrderCommandService CreateOrderCommandService() => new(
        UnitOfWork,
        Mapper,
        new MiniOrders.Application.Orders.Commands.CreateOrder.CreateOrderCommandValidator(),
        NullLogger<OrderCommandService>.Instance);

    public OrderQueryService CreateOrderQueryService() => new(UnitOfWork, Mapper);

    public async Task<Domain.Customers.Customer> AddCustomerAsync(
        string name = "Customer",
        string address = "1 Main Street",
        string phoneNumber = "555-0100")
    {
        var customer = new Domain.Customers.Customer
        {
            Id = Guid.NewGuid(),
            Name = name,
            Profile = new Domain.Customers.CustomerProfile
            {
                Id = Guid.NewGuid(),
                Address = address,
                PhoneNumber = phoneNumber
            }
        };
        customer.Profile.CustomerId = customer.Id;
        Context.Customers.Add(customer);
        await Context.SaveChangesAsync();
        return customer;
    }

    public async Task<Domain.Orders.Order> AddOrderAsync(
        Guid customerId,
        DateTime? orderDate = null,
        decimal totalAmount = 42.50m)
    {
        var order = new Domain.Orders.Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OrderDate = orderDate ?? DateTime.UtcNow,
            TotalAmount = totalAmount
        };
        Context.Orders.Add(order);
        await Context.SaveChangesAsync();
        return order;
    }

    public void Dispose() => Context.Dispose();
}