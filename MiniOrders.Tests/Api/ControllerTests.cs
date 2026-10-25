using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniOrders.Api.Controllers;
using MiniOrders.Api.Requests;
using MiniOrders.Application.Customers;
using MiniOrders.Application.Customers.Commands.CreateCustomer;
using MiniOrders.Application.Orders.Commands.CreateOrder;
using MiniOrders.Tests.TestSupport;

namespace MiniOrders.Tests.Api;

public sealed class ControllerTests
{
    [Fact]
    public async Task CustomersController_create_and_get_actions_return_expected_results()
    {
        using var database = new TestDatabase();
        var controller = new CustomersController(
            database.CreateCustomerCommandService(),
            database.CreateCustomerQueryService());

        var created = await controller.Create(
            new CreateCustomerCommand("Ada", "1 Main Street", "555-0100"), CancellationToken.None);
        var createdResult = Assert.IsType<ObjectResult>(created);
        Assert.Equal(201, createdResult.StatusCode);

        var id = await database.Context.Customers.Select(customer => customer.Id).SingleAsync();
        var byId = await controller.GetById(id, CancellationToken.None);
        Assert.IsType<OkObjectResult>(byId.Result);
        var all = await controller.GetAll(CancellationToken.None);
        var allResult = Assert.IsType<OkObjectResult>(all.Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<CustomerDto>>(allResult.Value));
    }

    [Fact]
    public async Task OrdersController_create_and_read_actions_return_expected_results()
    {
        using var database = new TestDatabase();
        var customer = await database.AddCustomerAsync();
        var controller = new OrdersController(
            database.CreateOrderCommandService(),
            database.CreateOrderQueryService());

        var created = await controller.Create(
            customer.Id,
            new CreateOrderRequest { OrderDate = DateTime.UtcNow, TotalAmount = 12.50m },
            CancellationToken.None);
        Assert.Equal(201, Assert.IsType<ObjectResult>(created).StatusCode);

        var byCustomer = await controller.GetByCustomerId(customer.Id, CancellationToken.None);
        var byCustomerResult = Assert.IsType<OkObjectResult>(byCustomer.Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<MiniOrders.Application.Orders.OrderDto>>(byCustomerResult.Value));
        var all = await controller.GetAll(CancellationToken.None);
        var allResult = Assert.IsType<OkObjectResult>(all.Result);
        Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<MiniOrders.Application.Orders.OrderDto>>(allResult.Value));
    }
}