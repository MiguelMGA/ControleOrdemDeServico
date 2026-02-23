using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.CreateCustomer;
using OsService.Services.V1.DeleteCustomer;
using OsService.Services.V1.GetCustomerById;
using OsService.Services.V1.SearchCustomer;

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

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? document,
        [FromQuery] string? phone,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(document) && string.IsNullOrWhiteSpace(phone))
            return BadRequest("Informe pelo menos um parâmetro: document ou phone.");

        var query = new SearchCustomerQuery(document?.Trim(), phone?.Trim());
        var result = await mediator.Send(query, ct);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCustomerCommand(id), ct);
        return NoContent();
    }
}