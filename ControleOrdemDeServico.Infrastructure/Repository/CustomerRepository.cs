using Dapper;
using OsService.Domain.Entities;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class CustomerRepository(IDefaultSqlConnectionFactory factory)
    : ICustomerRepository
{
    public async Task InsertAsync(CustomerEntity customer, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO dbo.Customers (Id, Name, Phone, Email, Document, CreatedAt)
            VALUES (@Id, @Name, @Phone, @Email, @Document, @CreatedAt);";

        using var conn = factory.Create();

        await conn.ExecuteAsync(
            new CommandDefinition(
                sql,
                new
                {
                    customer.Id,
                    customer.Name,
                    customer.Phone,
                    customer.Email,
                    customer.Document,
                    customer.CreatedAt
                },
                cancellationToken: ct));
    }

    public async Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
            SELECT Id,
                   Name,
                   Phone,
                   Email,
                   Document,
                   CreatedAt
            FROM dbo.Customers
            WHERE Id = @Id;";

        using var conn = factory.Create();

        var row = await conn.QuerySingleOrDefaultAsync(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: ct));

        if (row is null)
            return null;

        return CustomerEntity.Restore(
            row.Id,
            row.Name,
            row.Phone,
            row.Email,
            row.Document,
            row.CreatedAt
        );
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Id = @Id;";

        using var conn = factory.Create();

        var result = await conn.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: ct));

        return result.HasValue;
    }

    public async Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Document = @Document;";

        using var conn = factory.Create();

        var result = await conn.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(
                sql,
                new { Document = document },
                cancellationToken: ct));

        return result.HasValue;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Email = @Email;";

        using var conn = factory.Create();

        var result = await conn.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(
                sql,
                new { Email = email },
                cancellationToken: ct));

        return result.HasValue;
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Phone = @Phone;";

        using var conn = factory.Create();

        var result = await conn.QueryFirstOrDefaultAsync<int?>(
            new CommandDefinition(
                sql,
                new { Phone = phone },
                cancellationToken: ct));

        return result.HasValue;
    }
}