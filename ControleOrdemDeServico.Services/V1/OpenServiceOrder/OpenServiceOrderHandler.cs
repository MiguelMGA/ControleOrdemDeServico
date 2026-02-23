using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Entities;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using System.Text.Json;

namespace OsService.Services.V1.OpenServiceOrder;

public sealed class OpenServiceOrderHandler(
    ICustomerRepository customers,
    IServiceOrderRepository serviceOrders,
    IAuditLogRepository auditLogRepository,
    ILogger<OpenServiceOrderHandler> logger)
    : IRequestHandler<OpenServiceOrderCommand, (Guid Id, int Number)>
{
    public async Task<(Guid Id, int Number)> Handle(
        OpenServiceOrderCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando abertura de OS para cliente {CustomerId}",
            request.CustomerId);

        if (request.CustomerId == Guid.Empty)
        {
            logger.LogWarning(
                "Tentativa de abrir OS com CustomerId vazio");

            var serializedRequest = logger.IsEnabled(LogLevel.Warning)
                ? JsonSerializer.Serialize(request)
                : null;

            await auditLogRepository.AddAsync(
                entity: AuditEntities.ServiceOrder,
                entityId: null,
                action: "OpenFailed_InvalidCustomerId",
                data: serializedRequest,
                cancellationToken: cancellationToken);

            throw new DomainException("CustomerId é obrigatório.");
        }

        var exists = await customers.ExistsAsync(
            request.CustomerId,
            cancellationToken);

        if (!exists)
        {
            logger.LogWarning(
                "Tentativa de abrir OS para cliente inexistente {CustomerId}",
                request.CustomerId);

            var serializedRequest = logger.IsEnabled(LogLevel.Warning)
                ? JsonSerializer.Serialize(request)
                : null;

            await auditLogRepository.AddAsync(
                entity: AuditEntities.ServiceOrder,
                entityId: null,
                action: AuditActions.OpenFailedCustomerNotFound,
                data: serializedRequest,
                cancellationToken: cancellationToken);

            throw new DomainException("Customer não encontrado.");
        }

        var serviceOrder = ServiceOrderEntity.Create(
            request.CustomerId,
            request.Description);

        await serviceOrders.InsertAsync(serviceOrder, cancellationToken);

        logger.LogInformation(
            "OS criada com sucesso. Id: {ServiceOrderId}, Número: {Number}",
            serviceOrder.Id,
            serviceOrder.Number);

        var serializedServiceOrder = logger.IsEnabled(LogLevel.Information)
            ? JsonSerializer.Serialize(serviceOrder)
            : null;

        await auditLogRepository.AddAsync(
            entity: AuditEntities.ServiceOrder,
            entityId: serviceOrder.Id,
            action: AuditActions.Opened,
            data: serializedServiceOrder,
            cancellationToken: cancellationToken);

        return (serviceOrder.Id, serviceOrder.Number);
    }
}