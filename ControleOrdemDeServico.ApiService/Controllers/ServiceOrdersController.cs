using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.DeleteServiceOrder;
using OsService.Services.V1.GetAllServiceOrders;
using OsService.Services.V1.GetAllServiceOrdersIncludingDeleted;
using OsService.Services.V1.GetServiceOrderById;
using OsService.Services.V1.OpenServiceOrder;
using OsService.Services.V1.UpdateServiceOrderPrice;
using OsService.Services.V1.UpdateServiceOrderStatus;

namespace OsService.ApiService.Controllers;

/// <summary>
/// Responsável pelo gerenciamento de Ordens de Serviço.
/// Permite abertura, consulta, atualização de status, atualização de preço e exclusão.
/// </summary>
[ApiController]
[Route("v1/service-orders")]
[Produces("application/json")]
public sealed class ServiceOrdersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Abre uma nova Ordem de Serviço para um cliente.
    /// </summary>
    /// <param name="cmd">Dados necessários para abertura da Ordem de Serviço.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Identificador e número gerado da Ordem de Serviço.</returns>
    /// <response code="201">Ordem de Serviço criada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Obtém uma Ordem de Serviço pelo identificador.
    /// </summary>
    /// <param name="id">Identificador único da Ordem de Serviço.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Dados da Ordem de Serviço.</returns>
    /// <response code="200">Ordem de Serviço encontrada.</response>
    /// <response code="404">Ordem de Serviço não encontrada.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetServiceOrderByIdQuery(id), ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Atualiza o status de uma Ordem de Serviço.
    /// </summary>
    /// <param name="id">Identificador único da Ordem de Serviço.</param>
    /// <param name="cmd">Dados contendo o novo status.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Status atualizado com sucesso.</response>
    /// <response code="400">Id da URL diferente do corpo da requisição ou dados inválidos.</response>
    /// <response code="404">Ordem de Serviço não encontrada.</response>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Atualiza o preço de uma Ordem de Serviço.
    /// </summary>
    /// <param name="id">Identificador único da Ordem de Serviço.</param>
    /// <param name="cmd">Dados contendo o novo valor.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Preço atualizado com sucesso.</response>
    /// <response code="400">Id da URL diferente do corpo da requisição ou valor inválido.</response>
    /// <response code="404">Ordem de Serviço não encontrada.</response>
    [HttpPut("{id:guid}/price")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Remove logicamente uma Ordem de Serviço.
    /// </summary>
    /// <param name="id">Identificador único da Ordem de Serviço.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Ordem de Serviço removida com sucesso.</response>
    /// <response code="404">Ordem de Serviço não encontrada.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteServiceOrderCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Retorna todas as Ordens de Serviço ativas.
    /// Pode ser filtrado por CustomerId.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? customerId,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetAllServiceOrdersQuery(customerId), ct);

        return Ok(result);
    }

    /// <summary>
    /// SOMENTE PARA TESTES.
    /// Retorna todas as Ordens de Serviço, inclusive excluídas.
    /// Pode ser filtrado por CustomerId.
    /// </summary>
    [HttpGet("all-with-deleted")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllIncludingDeleted(
        [FromQuery] Guid? customerId,
        CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetAllServiceOrdersIncludingDeletedQuery(customerId), ct);

        return Ok(result);
    }
}