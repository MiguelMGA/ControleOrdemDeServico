using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.DeleteServiceOrder;
using OsService.Services.V1.GetServiceOrderById;
using OsService.Services.V1.OpenServiceOrder;
using OsService.Services.V1.UpdateServiceOrderPrice;
using OsService.Services.V1.UpdateServiceOrderStatus;

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

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateServiceOrderStatusCommand cmd,
        CancellationToken ct)
    {
        if (id != cmd.Id)
            return BadRequest("O Id da URL difere do corpo da requisição.");

        await mediator.Send(cmd, ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/price")]
    public async Task<IActionResult> UpdatePrice(
        Guid id,
        [FromBody] UpdateServiceOrderPriceCommand cmd,
        CancellationToken ct)
    {
        if (id != cmd.Id)
            return BadRequest("O Id da URL difere do corpo da requisição.");

        await mediator.Send(cmd, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteServiceOrderCommand(id), ct);
        return NoContent();
    }
}