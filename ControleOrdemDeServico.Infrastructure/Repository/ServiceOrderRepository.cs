using Dapper;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Repository;

public sealed class ServiceOrderRepository(IDefaultSqlConnectionFactory factory)
    : IServiceOrderRepository
{
    public async Task InsertAsync(ServiceOrderEntity entity, CancellationToken ct)
    {
        const string sql = @"
            INSERT INTO dbo.ServiceOrders
                (Id, CustomerId, Description, Status, OpenedAt, Price, Coin, IsDeleted, DeletedAt)
            OUTPUT INSERTED.Number
            VALUES
                (@Id, @CustomerId, @Description, @Status, @OpenedAt, @Price, @Coin, @IsDeleted, @DeletedAt);";

        using var conn = factory.Create();

        var number = await conn.ExecuteScalarAsync<int>(
            new CommandDefinition(sql, new
            {
                entity.Id,
                entity.CustomerId,
                entity.Description,
                Status = (int)entity.Status,
                entity.OpenedAt,
                entity.Price,
                Coin = entity.Currency.Code,
                IsDeleted = entity.IsDeleted(),
                entity.DeletedAt
            }, cancellationToken: ct));

        entity.SetNumber(number);
    }

    public async Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Number,
               CustomerId,
               Description,
               Status,
               OpenedAt,
               StartedAt,
               FinishedAt,
               Price,
               Coin,
               UpdatedPriceAt,
               IsDeleted,
               DeletedAt
        FROM dbo.ServiceOrders
        WHERE Id = @Id
          AND IsDeleted = 0;";

        using var conn = factory.Create();

        var row = await conn.QuerySingleOrDefaultAsync(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: ct));

        if (row is null)
            return null;

        return ServiceOrderEntity.Restore(
            new ServiceOrderEntity.Snapshot(
                row.Id,
                row.Number,
                row.CustomerId,
                row.Description,
                (ServiceOrderStatus)row.Status,
                row.OpenedAt,
                row.StartedAt,
                row.FinishedAt,
                row.Price,
                row.Coin,
                row.UpdatedPriceAt,
                row.IsDeleted,
                row.DeletedAt
            ));
    }

    public async Task UpdateAsync(ServiceOrderEntity entity, CancellationToken ct)
    {
        const string sql = @"
        UPDATE dbo.ServiceOrders
        SET Status = @Status,
            StartedAt = @StartedAt,
            FinishedAt = @FinishedAt,
            Price = @Price,
            UpdatedPriceAt = @UpdatedPriceAt,
            IsDeleted = @IsDeleted,
            DeletedAt = @DeletedAt
        WHERE Id = @Id;";

        using var conn = factory.Create();

        await conn.ExecuteAsync(new CommandDefinition(sql, new
        {
            entity.Id,
            Status = (int)entity.Status,
            entity.StartedAt,
            entity.FinishedAt,
            entity.Price,
            entity.UpdatedPriceAt,
            IsDeleted = entity.IsDeleted(),
            entity.DeletedAt
        }, cancellationToken: ct));
    }

    public async Task<IEnumerable<ServiceOrderEntity>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct)
    {
        const string sql = @"
            SELECT Id,
                   Number,
                   CustomerId,
                   Description,
                   Status,
                   OpenedAt,
                   StartedAt,
                   FinishedAt,
                   Price,
                   Coin,
                   UpdatedPriceAt,
                   IsDeleted,
                   DeletedAt
            FROM dbo.ServiceOrders
            WHERE CustomerId = @CustomerId;";

        using var conn = factory.Create();

        var rows = await conn.QueryAsync(sql, new { CustomerId = customerId });

        var result = new List<ServiceOrderEntity>();

        foreach (var row in rows)
        {
            result.Add(ServiceOrderEntity.Restore(
                new ServiceOrderEntity.Snapshot(
                    row.Id,
                    row.Number,
                    row.CustomerId,
                    row.Description,
                    (ServiceOrderStatus)row.Status,
                    row.OpenedAt,
                    row.StartedAt,
                    row.FinishedAt,
                    row.Price,
                    row.Coin,
                    row.UpdatedPriceAt,
                    row.IsDeleted,
                    row.DeletedAt
                )));
        }

        return result;
    }

    public async Task<IEnumerable<ServiceOrderEntity>> GetAllAsync(
    Guid? customerId,
    CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Number,
               CustomerId,
               Description,
               Status,
               OpenedAt,
               StartedAt,
               FinishedAt,
               Price,
               Coin,
               UpdatedPriceAt,
               IsDeleted,
               DeletedAt
        FROM dbo.ServiceOrders
        WHERE IsDeleted = 0
          AND (@CustomerId IS NULL OR CustomerId = @CustomerId);";

        using var conn = factory.Create();

        var rows = await conn.QueryAsync(
            new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: ct));

        return rows.Select(Map);
    }

    public async Task<IEnumerable<ServiceOrderEntity>> GetAllIncludingDeletedAsync(
    Guid? customerId,
    CancellationToken ct)
    {
        const string sql = @"
        SELECT Id,
               Number,
               CustomerId,
               Description,
               Status,
               OpenedAt,
               StartedAt,
               FinishedAt,
               Price,
               Coin,
               UpdatedPriceAt,
               IsDeleted,
               DeletedAt
        FROM dbo.ServiceOrders
        WHERE (@CustomerId IS NULL OR CustomerId = @CustomerId);";

        using var conn = factory.Create();

        var rows = await conn.QueryAsync(
            new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: ct));

        return rows.Select(Map);
    }

    private static ServiceOrderEntity Map(dynamic row)
    {
        return ServiceOrderEntity.Restore(
            new ServiceOrderEntity.Snapshot(
                row.Id,
                row.Number,
                row.CustomerId,
                row.Description,
                (ServiceOrderStatus)row.Status,
                row.OpenedAt,
                row.StartedAt,
                row.FinishedAt,
                row.Price,
                row.Coin,
                row.UpdatedPriceAt,
                row.IsDeleted,
                row.DeletedAt
            ));
    }
}