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

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct)
    {
        var customer = await _queryService.GetCustomerByIdAsync(id, ct);
        if (customer is null)
            return NotFound();

        return Ok(customer);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerDto>>> GetAll(CancellationToken ct)
    {
        var customers = await _queryService.GetAllCustomersAsync(ct);
        return Ok(customers);
    }
}
