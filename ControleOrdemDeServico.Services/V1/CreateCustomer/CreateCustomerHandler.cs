using MediatR;
using Microsoft.Extensions.Logging;
using OsService.Domain.Entities;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using System.Text.Json;

namespace OsService.Services.V1.CreateCustomer;

public sealed class CreateCustomerHandler(
    ICustomerRepository repo,
    IAuditLogRepository auditLogRepository,
    ILogger<CreateCustomerHandler> logger)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Iniciando criação de cliente. Nome: {Name}, Documento: {Document}",
            request.Name,
            request.Document);

        var document = request.Document?.Trim();
        var email = request.Email?.Trim();
        var phone = request.Phone?.Trim();

        if (!string.IsNullOrWhiteSpace(document) &&
            await repo.ExistsByDocumentAsync(document, cancellationToken))
        {
            logger.LogWarning(
                "Tentativa de criar cliente com documento duplicado: {Document}",
                document);

            string? serializedRequest = null;

            if (logger.IsEnabled(LogLevel.Warning))
            {
                serializedRequest = JsonSerializer.Serialize(request);
            }

            await auditLogRepository.AddAsync(
                entity: AuditEntities.Customer,
                entityId: null,
                action: AuditActions.CreateFailedDuplicateDocument,
                data: serializedRequest,
                cancellationToken: cancellationToken);

            throw new DomainException("Já existe cliente com o mesmo documento.");
        }

        if (!string.IsNullOrWhiteSpace(email) &&
            await repo.ExistsByEmailAsync(email, cancellationToken))
        {
            logger.LogWarning(
                "Tentativa de criar cliente com email duplicado: {Email}",
                email);

            string? serializedRequest = null;

            if (logger.IsEnabled(LogLevel.Warning))
            {
                serializedRequest = JsonSerializer.Serialize(request);
            }

            await auditLogRepository.AddAsync(
                entity: AuditEntities.Customer,
                entityId: null,
                action: AuditActions.CreateFailedDuplicateEmail,
                data: serializedRequest,
                cancellationToken: cancellationToken);

            throw new DomainException("Já existe cliente com o mesmo email.");
        }

        if (!string.IsNullOrWhiteSpace(phone) &&
            await repo.ExistsByPhoneAsync(phone, cancellationToken))
        {
            logger.LogWarning(
                "Tentativa de criar cliente com telefone duplicado: {Phone}",
                phone);

            string? serializedRequest = null;

            if (logger.IsEnabled(LogLevel.Warning))
            {
                serializedRequest = JsonSerializer.Serialize(request);
            }

            await auditLogRepository.AddAsync(
                entity: AuditEntities.Customer,
                entityId: null,
                action: AuditActions.CreateFailedDuplicatePhone,
                data: serializedRequest,
                cancellationToken: cancellationToken);

            throw new DomainException("Já existe cliente com o mesmo telefone.");
        }

        var customer = CustomerEntity.Create(
            request.Name,
            phone,
            email,
            document);

        await repo.InsertAsync(customer, cancellationToken);

        logger.LogInformation(
            "Cliente criado com sucesso. Id: {CustomerId}, Nome: {Name}",
            customer.Id,
            customer.Name);

        string? serializedCustomer = null;

        if (logger.IsEnabled(LogLevel.Information))
        {
            serializedCustomer = JsonSerializer.Serialize(customer);
        }

        await auditLogRepository.AddAsync(
            entity: AuditEntities.Customer,
            entityId: customer.Id,
            action: AuditActions.Created,
            data: serializedCustomer,
            cancellationToken: cancellationToken);

        return customer.Id;
    }
}