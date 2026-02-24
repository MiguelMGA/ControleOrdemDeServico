using MediatR;
using OsService.Domain.Enums;

namespace OsService.Services.V1.UpdateServiceOrderStatus;

/// <summary>
/// Comando responsável por alterar o status de uma ordem de serviço.
/// </summary>
/// <param name="Id">
/// Identificador único da ordem de serviço que terá o status atualizado.
/// </param>
/// <param name="NewStatus">
/// Novo status desejado para a ordem de serviço.
/// A transição deve respeitar as regras de negócio definidas no domínio.
/// </param>
public sealed record UpdateServiceOrderStatusCommand(
    Guid Id,
    ServiceOrderStatus NewStatus
) : IRequest<Unit>;