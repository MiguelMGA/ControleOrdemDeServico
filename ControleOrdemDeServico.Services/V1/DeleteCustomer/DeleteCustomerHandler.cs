using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using System.Text.Json;

namespace OsService.Services.V1.DeleteCustomer
{
    public sealed class DeleteCustomerHandler(
        ICustomerRepository customerRepo,
        IServiceOrderRepository serviceOrderRepo,
        IAuditLogRepository auditLogRepo,
        ILogger<DeleteCustomerHandler> logger)
        : IRequestHandler<DeleteCustomerCommand, Unit>
    {
        public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await customerRepo.GetByIdAsync(request.Id, cancellationToken)
                           ?? throw new DomainException($"Customer {request.Id} não encontrado.");

            if (customer.IsDeleted)
                throw new DomainException($"Customer {request.Id} já está excluído.");

            customer.MarkAsDeleted();
            await customerRepo.UpdateAsync(customer, cancellationToken);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Customer {CustomerId} excluído logicamente.", customer.Id);

                var customerJson = JsonSerializer.Serialize(customer);
                await auditLogRepo.AddAsync(
                    entity: "Customer",
                    entityId: customer.Id,
                    action: "Deleted",
                    data: customerJson,
                    cancellationToken: cancellationToken);
            }

            var serviceOrders = await serviceOrderRepo.GetByCustomerIdAsync(customer.Id, cancellationToken);

            foreach (var so in serviceOrders.Where(so => !so.IsDeleted()))
            {
                so.MarkAsDeleted();
                await serviceOrderRepo.UpdateAsync(so, cancellationToken);

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation(
                        "ServiceOrder {ServiceOrderId} de Customer {CustomerId} excluída logicamente.",
                        so.Id, customer.Id);

                    var soJson = JsonSerializer.Serialize(so);
                    await auditLogRepo.AddAsync(
                        entity: "ServiceOrder",
                        entityId: so.Id,
                        action: "DeletedByCustomer",
                        data: soJson,
                        cancellationToken: cancellationToken);
                }
            }

            return Unit.Value;
        }
    }
}