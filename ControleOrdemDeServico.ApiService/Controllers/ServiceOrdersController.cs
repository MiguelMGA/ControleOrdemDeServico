using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.GetServiceOrderById;
using OsService.Services.V1.OpenServiceOrder;

namespace OsService.ApiService.Controllers;

[ApiController]
[Route("v1/service-orders")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Open(
        [FromBody] OpenServiceOrderCommand cmd,
        CancellationToken ct)
    {
        var (id, number) = await mediator.Send(cmd, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            new { id, number });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetServiceOrderByIdQuery(id), ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }
}