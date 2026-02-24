using MediatR;

namespace OsService.Services.V1.OpenServiceOrder;

/// <summary>
/// Comando responsável por abrir uma nova Ordem de Serviço para um cliente.
/// </summary>
/// <param name="CustomerId">
/// Identificador único do cliente para o qual a Ordem de Serviço será aberta.
/// </param>
/// <param name="Description">
/// Descrição detalhada do serviço a ser executado.
/// </param>
/// <remarks>
/// Retorna uma tupla contendo:
/// - Id: Identificador único da Ordem de Serviço criada.
/// - Number: Número sequencial gerado para a Ordem de Serviço.
/// </remarks>
public sealed record OpenServiceOrderCommand(
    Guid CustomerId,
    string Description
) : IRequest<(Guid Id, int Number)>;