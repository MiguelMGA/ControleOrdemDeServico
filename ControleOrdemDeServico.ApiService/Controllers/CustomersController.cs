using MediatR;
using Microsoft.AspNetCore.Mvc;
using OsService.Services.V1.CreateCustomer;
using OsService.Services.V1.DeleteCustomer;
using OsService.Services.V1.GetAllCustomers;
using OsService.Services.V1.GetAllCustomersIncludingDeleted;
using OsService.Services.V1.GetCustomerById;
using OsService.Services.V1.SearchCustomer;

namespace OsService.ApiService.Controllers;

/// <summary>
/// Responsável pelo gerenciamento de clientes.
/// Permite criar, consultar, pesquisar e excluir clientes.
/// </summary>
[ApiController]
[Route("v1/customers")]
[Produces("application/json")]
public sealed class CustomersController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Cria um novo cliente.
    /// </summary>
    /// <param name="cmd">Dados necessários para criação do cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Retorna o identificador do cliente criado.</returns>
    /// <response code="201">Cliente criado com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Obtém um cliente pelo identificador.
    /// </summary>
    /// <param name="id">Identificador único do cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Dados do cliente.</returns>
    /// <response code="200">Cliente encontrado.</response>
    /// <response code="404">Cliente não encontrado.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCustomerByIdQuery(id), ct);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Pesquisa clientes por documento ou telefone.
    /// Pelo menos um dos parâmetros deve ser informado.
    /// </summary>
    /// <param name="document">Documento do cliente (CPF/CNPJ).</param>
    /// <param name="phone">Telefone do cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de clientes encontrados.</returns>
    /// <response code="200">Consulta realizada com sucesso.</response>
    /// <response code="400">Nenhum parâmetro informado.</response>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Remove logicamente um cliente pelo identificador.
    /// </summary>
    /// <param name="id">Identificador único do cliente.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Cliente removido com sucesso.</response>
    /// <response code="404">Cliente não encontrado.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteCustomerCommand(id), ct);
        return NoContent();
    }

    /// <summary>
    /// Retorna todos os clientes ativos (não excluídos).
    /// </summary>
    /// <response code="200">Lista de clientes ativos.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllCustomersQuery(), ct);
        return Ok(result);
    }

    /// <summary>
    /// SOMENTE PARA TESTES.
    /// Retorna todos os clientes, inclusive os excluídos.
    /// NÃO utilizar em produção.
    /// </summary>
    /// <response code="200">Lista completa de clientes.</response>
    [HttpGet("all-with-deleted")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllIncludingDeleted(CancellationToken ct)
    {
        var result = await mediator.Send(
            new GetAllCustomersIncludingDeletedQuery(), ct);

        return Ok(result);
    }
}