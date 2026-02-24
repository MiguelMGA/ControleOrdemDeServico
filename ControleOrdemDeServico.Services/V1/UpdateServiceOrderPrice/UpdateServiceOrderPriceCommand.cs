using MediatR;

namespace OsService.Services.V1.UpdateServiceOrderPrice;

/// <summary>
/// Comando responsável por atualizar o preço de uma ordem de serviço existente.
/// </summary>
/// <param name="Id">
/// Identificador único da ordem de serviço que terá o preço atualizado.
/// </param>
/// <param name="Price">
/// Novo valor monetário da ordem de serviço. 
/// Deve ser maior ou igual a zero.
/// </param>
public sealed record UpdateServiceOrderPriceCommand(
    Guid Id,
    decimal Price
) : IRequest<Unit>;