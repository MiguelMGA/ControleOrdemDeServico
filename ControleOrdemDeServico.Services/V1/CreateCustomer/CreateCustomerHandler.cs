using MediatR;
using OsService.Domain.Entities;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.CreateCustomer;

public sealed class CreateCustomerHandler(ICustomerRepository repo)
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var document = request.Document?.Trim();
        var email = request.Email?.Trim();
        var phone = request.Phone?.Trim();

        if (!string.IsNullOrWhiteSpace(document) &&
            await repo.ExistsByDocumentAsync(document, cancellationToken))
        {
            throw new DomainException("Já existe cliente com o mesmo documento.");
        }

        if (!string.IsNullOrWhiteSpace(email) &&
            await repo.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new DomainException("Já existe cliente com o mesmo email.");
        }

        if (!string.IsNullOrWhiteSpace(phone) &&
            await repo.ExistsByPhoneAsync(phone, cancellationToken))
        {
            throw new DomainException("Já existe cliente com o mesmo telefone.");
        }

        var customer = CustomerEntity.Create(
            request.Name,
            phone,
            email,
            document);

        await repo.InsertAsync(customer, cancellationToken);

        return customer.Id;
    }
}
