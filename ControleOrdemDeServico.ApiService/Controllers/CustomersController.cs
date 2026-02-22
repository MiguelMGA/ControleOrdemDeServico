using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.CreateCustomer;
using OsService.Services.V1.GetCustomerById;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/customers")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerCommand cmd,
        CancellationToken ct)
    {
        var id = await mediator.Send(cmd, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}