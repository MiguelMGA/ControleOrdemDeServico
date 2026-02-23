using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Enums;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using System.Text.Json;

namespace OsService.Services.V1.UpdateServiceOrderStatus;

public sealed class UpdateServiceOrderStatusHandler(
    IServiceOrderRepository serviceOrders,
    IAuditLogRepository auditLogRepository,
    ILogger<UpdateServiceOrderStatusHandler> logger
) : IRequestHandler<UpdateServiceOrderStatusCommand, Unit>
{
    public async Task<Unit> Handle(UpdateServiceOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var os = await serviceOrders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("Ordem de serviço não encontrada.");

        switch (request.NewStatus)
        {
            case ServiceOrderStatus.InProgress:
                os.Start();
                break;
            case ServiceOrderStatus.Finished:
                os.Finish();
                break;
            case ServiceOrderStatus.Open:
                throw new DomainException("Não é permitido reabrir a OS finalizada.");
            default:
                throw new DomainException($"Status inválido: {request.NewStatus}");
        }

        await serviceOrders.UpdateAsync(os, cancellationToken);

        var data = logger.IsEnabled(LogLevel.Information)
            ? JsonSerializer.Serialize(new { os.Id, os.Status, os.StartedAt, os.FinishedAt })
            : null;

        await auditLogRepository.AddAsync(
            entity: AuditEntities.ServiceOrder,
            entityId: os.Id,
            action: "StatusUpdated",
            data: data,
            cancellationToken: cancellationToken
        );

        return Unit.Value;
    }
}