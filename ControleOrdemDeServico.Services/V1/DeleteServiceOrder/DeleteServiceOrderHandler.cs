using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using System.Text.Json;

namespace OsService.Services.V1.DeleteServiceOrder
{
    public sealed class DeleteServiceOrderHandler(
        IServiceOrderRepository serviceOrderRepo,
        IAuditLogRepository auditLogRepo,
        ILogger<DeleteServiceOrderHandler> logger)
        : IRequestHandler<DeleteServiceOrderCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteServiceOrderCommand request, CancellationToken cancellationToken)
        {
            var so = await serviceOrderRepo.GetByIdAsync(request.Id, cancellationToken);

            if (so is null)
                throw new DomainException($"ServiceOrder {request.Id} não encontrada.");

            if (so.IsDeleted())
                throw new DomainException($"ServiceOrder {request.Id} já está excluída.");

            so.MarkAsDeleted();

            await serviceOrderRepo.UpdateAsync(so, cancellationToken);

            logger.LogInformation("ServiceOrder {ServiceOrderId} excluída logicamente.", so.Id);

            await auditLogRepo.AddAsync(
                entity: "ServiceOrder",
                entityId: so.Id,
                action: "Deleted",
                data: JsonSerializer.Serialize(so),
                cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}