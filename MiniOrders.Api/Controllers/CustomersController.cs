using Microsoft.AspNetCore.Mvc;
using MiniOrders.Application.Customers;
using MiniOrders.Application.Customers.Commands.CreateCustomer;

namespace MiniOrders.Api.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerCommandService _commandService;
    private readonly ICustomerQueryService _queryService;
    public CustomersController(ICustomerCommandService commandService, ICustomerQueryService queryService)
    {
        _commandService = commandService;
        _queryService = queryService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateCustomerCommand command, CancellationToken ct)
    {
        var id = await _commandService.CreateCustomerAsync(command, ct);
        return StatusCode(StatusCodes.Status201Created, new { id });
    }
}
