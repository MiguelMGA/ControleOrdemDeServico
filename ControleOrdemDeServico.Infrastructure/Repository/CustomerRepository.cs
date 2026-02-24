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
            INSERT INTO dbo.Customers 
                (Id, Name, Phone, Email, Document, CreatedAt)
            VALUES 
                (@Id, @Name, @Phone, @Email, @Document, @CreatedAt);";

        using var conn = factory.Create();
        await conn.ExecuteAsync(
            new CommandDefinition(sql, new
            {
                customer.Id,
                customer.Name,
                customer.Phone,
                customer.Email,
                customer.Document,
                customer.CreatedAt
            }, cancellationToken: ct));
    }

    public async Task<CustomerEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Name,
               Phone,
               Email,
               Document,
               CreatedAt,
               IsDeleted,
               DeletedAt
        FROM dbo.Customers
        WHERE Id = @Id
          AND IsDeleted = 0;";

        using var conn = factory.Create();
        var row = await conn.QuerySingleOrDefaultAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        if (row is null) return null;

        var snapshot = new CustomerSnapshot
        {
            Id = row.Id,
            Name = row.Name,
            Phone = row.Phone,
            Email = row.Email,
            Document = row.Document,
            CreatedAt = row.CreatedAt,
            IsDeleted = row.IsDeleted,
            DeletedAt = row.DeletedAt
        };

        return CustomerEntity.Restore(snapshot);
    }

    public async Task UpdateAsync(CustomerEntity customer, CancellationToken ct)
    {
        const string sql = @"
            UPDATE dbo.Customers
            SET Name = @Name,
                Phone = @Phone,
                Email = @Email,
                Document = @Document,
                IsDeleted = @IsDeleted,
                DeletedAt = @DeletedAt
            WHERE Id = @Id;";

        using var conn = factory.Create();
        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.Document,
            customer.IsDeleted,
            customer.DeletedAt
        }, cancellationToken: ct));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Id = @Id;";
        using var conn = factory.Create();
        var result = await conn.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));
        return result.HasValue;
    }

    public async Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Document = @Document;";
        using var conn = factory.Create();
        var result = await conn.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { Document = document }, cancellationToken: ct));
        return result.HasValue;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Email = @Email;";
        using var conn = factory.Create();
        var result = await conn.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { Email = email }, cancellationToken: ct));
        return result.HasValue;
    }

    public async Task<bool> ExistsByPhoneAsync(string phone, CancellationToken ct)
    {
        const string sql = "SELECT 1 FROM dbo.Customers WHERE Phone = @Phone;";
        using var conn = factory.Create();
        var result = await conn.QueryFirstOrDefaultAsync<int?>(new CommandDefinition(sql, new { Phone = phone }, cancellationToken: ct));
        return result.HasValue;
    }

    public async Task<IEnumerable<CustomerEntity>> SearchAsync(string? document, string? phone, CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Name,
               Phone,
               Email,
               Document,
               CreatedAt,
               IsDeleted,
               DeletedAt
        FROM dbo.Customers
        WHERE (@Document IS NULL OR Document LIKE @Document)
          AND (@Phone IS NULL OR Phone LIKE @Phone)
          AND IsDeleted = 0;";

        using var conn = factory.Create();

        var rows = await conn.QueryAsync(
            new CommandDefinition(sql, new
            {
                Document = string.IsNullOrWhiteSpace(document) ? null : $"%{document.Trim()}%",
                Phone = string.IsNullOrWhiteSpace(phone) ? null : $"%{phone.Trim()}%"
            }, cancellationToken: ct));

        var result = new List<CustomerEntity>();

        foreach (var row in rows)
        {
            var snapshot = new CustomerSnapshot
            {
                Id = row.Id,
                Name = row.Name,
                Phone = row.Phone,
                Email = row.Email,
                Document = row.Document,
                CreatedAt = row.CreatedAt,
                IsDeleted = row.IsDeleted,
                DeletedAt = row.DeletedAt
            };

            var entity = CustomerEntity.Restore(snapshot);
            result.Add(entity);
        }

        return result;
    }

    public async Task<IEnumerable<CustomerEntity>> GetAllAsync(CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Name,
               Phone,
               Email,
               Document,
               CreatedAt,
               IsDeleted,
               DeletedAt
        FROM dbo.Customers
        WHERE IsDeleted = 0;";

        using var conn = factory.Create();

        var rows = await conn.QueryAsync(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.Select(row =>
            CustomerEntity.Restore(new CustomerSnapshot
            {
                Id = row.Id,
                Name = row.Name,
                Phone = row.Phone,
                Email = row.Email,
                Document = row.Document,
                CreatedAt = row.CreatedAt,
                IsDeleted = row.IsDeleted,
                DeletedAt = row.DeletedAt
            }));
    }

    public async Task<IEnumerable<CustomerEntity>> GetAllIncludingDeletedAsync(CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Name,
               Phone,
               Email,
               Document,
               CreatedAt,
               IsDeleted,
               DeletedAt
        FROM dbo.Customers;";

        using var conn = factory.Create();

        var rows = await conn.QueryAsync(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.Select(row =>
            CustomerEntity.Restore(new CustomerSnapshot
            {
                Id = row.Id,
                Name = row.Name,
                Phone = row.Phone,
                Email = row.Email,
                Document = row.Document,
                CreatedAt = row.CreatedAt,
                IsDeleted = row.IsDeleted,
                DeletedAt = row.DeletedAt
            }));
    }
}