using Microsoft.AspNetCore.Mvc;
using MiniOrders.Api.Requests;
using MiniOrders.Application.Orders;
using MiniOrders.Application.Orders.Commands.CreateOrder;

namespace MiniOrders.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:guid}/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderCommandService _commandService;
    private readonly IOrderQueryService _queryService;
    public OrdersController(IOrderCommandService commandService, IOrderQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid customerId, CreateOrderRequest createOrderRequest, CancellationToken ct)
    {
        var id = await _commandService.CreateOrderAsync(
            new CreateOrderCommand(customerId, createOrderRequest.OrderDate, createOrderRequest.TotalAmount), ct);

        return StatusCode(StatusCodes.Status201Created, new { id });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetByCustomerId(Guid customerId, CancellationToken ct)
    {
        return Ok(await _queryService.GetOrdersByCustomerIdAsync(customerId, ct));
    }

    [HttpGet("~/api/orders")]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _queryService.GetAllOrdersAsync(ct));
    }
}
