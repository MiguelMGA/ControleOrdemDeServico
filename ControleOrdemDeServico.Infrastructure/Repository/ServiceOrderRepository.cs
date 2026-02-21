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
                (Id, CustomerId, Description, Status, OpenedAt, Price, Coin)
            OUTPUT INSERTED.Number
            VALUES
                (@Id, @CustomerId, @Description, @Status, @OpenedAt, @Price, @Coin);";

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
                Coin = entity.Currency.Code
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
                   Price,
                   Coin,
                   UpdatedPriceAt
            FROM dbo.ServiceOrders
            WHERE Id = @Id;";

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
                row.Price,
                row.Coin,
                row.UpdatedPriceAt
            ));
    }
}