using FluentValidation;
using MiniOrders.Application.Exceptions;
using MiniOrders.Application.Orders.Commands.CreateOrder;
using MiniOrders.Tests.TestSupport;

namespace MiniOrders.Tests.Application;

public sealed class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_persists_order_for_existing_customer()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync();
        var date = new DateTime(2026, 10, 25, 9, 30, 0, DateTimeKind.Utc);
        var command = new CreateOrderCommand(customer.Id, date, 123.45m);

        var id = await database.CreateOrderCommandService().CreateOrderAsync(command, CancellationToken.None);
        var order = await database.Context.Orders.FindAsync(id);

        Assert.NotNull(order);
        Assert.Equal(customer.Id, order.CustomerId);
        Assert.Equal(date, order.OrderDate);
        Assert.Equal(123.45m, order.TotalAmount);
    }

    [Fact]
    public async Task CreateOrderAsync_rejects_missing_customer()
    {
        using var database = new TestDatabase();
        var customerId = Guid.NewGuid();

        await Assert.ThrowsAsync<NotFoundException>(() => database.CreateOrderCommandService()
            .CreateOrderAsync(new CreateOrderCommand(customerId, DateTime.UtcNow, 1m), CancellationToken.None));

        Assert.Empty(database.Context.Orders);
    }

    [Theory]
    [InlineData("empty-customer")]
    [InlineData("empty-date")]
    [InlineData("zero-total")]
    [InlineData("negative-total")]
    public async Task CreateOrderAsync_rejects_invalid_values(string invalidValue)
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync();
        var command = invalidValue switch
        {
            "empty-customer" => new CreateOrderCommand(Guid.Empty, DateTime.UtcNow, 1m),
            "empty-date" => new CreateOrderCommand(customer.Id, default, 1m),
            "zero-total" => new CreateOrderCommand(customer.Id, DateTime.UtcNow, 0m),
            _ => new CreateOrderCommand(customer.Id, DateTime.UtcNow, -1m)
        };

        await Assert.ThrowsAsync<ValidationException>(() => database.CreateOrderCommandService()
            .CreateOrderAsync(command, CancellationToken.None));

        Assert.Empty(database.Context.Orders);
    }

    [Fact]
    public async Task GetOrdersByCustomerIdAsync_returns_only_that_customers_orders()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync("One");
        var other = await database.AddCustomerAsync("Two");
        var order = await database.AddOrderAsync(customer.Id);
        await database.AddOrderAsync(other.Id);

        var result = await database.CreateOrderQueryService()
            .GetOrdersByCustomerIdAsync(customer.Id, CancellationToken.None);

        var item = Assert.Single(result);
        Assert.Equal(order.Id, item.Id);
        Assert.Equal(customer.Id, item.CustomerId);
    }

    [Fact]
    public async Task GetOrdersByCustomerIdAsync_rejects_unknown_customer()
    {
        using var database = new TestDatabase();
        var id = Guid.NewGuid();

        await Assert.ThrowsAsync<NotFoundException>(() => database.CreateOrderQueryService()
            .GetOrdersByCustomerIdAsync(id, CancellationToken.None));
    }

    [Fact]
    public async Task GetAllOrdersAsync_returns_all_orders()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync();
        await database.AddOrderAsync(customer.Id);
        await database.AddOrderAsync(customer.Id);

        var result = await database.CreateOrderQueryService().GetAllOrdersAsync(CancellationToken.None);

        Assert.Equal(2, result.Count);
    }
}