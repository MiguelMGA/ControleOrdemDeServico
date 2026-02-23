using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using System.Text.Json;

namespace OsService.Services.V1.UpdateServiceOrderPrice;

public sealed class UpdateServiceOrderPriceHandler(
    IServiceOrderRepository serviceOrders,
    IAuditLogRepository auditLogRepository,
    ILogger<UpdateServiceOrderPriceHandler> logger
) : IRequestHandler<UpdateServiceOrderPriceCommand, Unit>
{
    public async Task<Unit> Handle(UpdateServiceOrderPriceCommand request, CancellationToken cancellationToken)
    {
        var os = await serviceOrders.GetByIdAsync(request.Id, cancellationToken)
                 ?? throw new DomainException("Ordem de serviço não encontrada.");

        os.UpdatePrice(request.Price);

        await serviceOrders.UpdateAsync(os, cancellationToken);

        var data = logger.IsEnabled(LogLevel.Information)
            ? JsonSerializer.Serialize(new { os.Id, os.Price, os.UpdatedPriceAt })
            : null;

        await auditLogRepository.AddAsync(
            entity: AuditEntities.ServiceOrder,
            entityId: os.Id,
            action: "PriceUpdated",
            data: data,
            cancellationToken: cancellationToken
        );

        return Unit.Value;
    }
}