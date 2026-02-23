using OsService.Domain.Entities;

namespace OsService.Infrastructure.Repository;

public interface ICustomerRepository
{
    Task InsertAsync(CustomerEntity customer, CancellationToken ct);
    Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(CustomerEntity customer, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);

    // Validações de duplicidade
    Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct);

    // Pesquisa de clientes
    Task<IEnumerable<CustomerEntity>> SearchAsync(string? document, string? phone, CancellationToken ct);
}