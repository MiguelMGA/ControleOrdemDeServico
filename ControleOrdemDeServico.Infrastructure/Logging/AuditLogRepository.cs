using Dapper;
using OsService.Infrastructure.Databases;

namespace OsService.Infrastructure.Logging
{
    public sealed class AuditLogRepository(
    IDefaultSqlConnectionFactory connectionFactory)
    : IAuditLogRepository
    {
        public async Task AddAsync(
            string entity,
            Guid? entityId,
            string action,
            string? data,
            CancellationToken cancellationToken)
        {
            const string sql = """
        INSERT INTO dbo.AuditLogs
        (Id, Entity, EntityId, Action, Data, CreatedAt)
        VALUES
        (@Id, @Entity, @EntityId, @Action, @Data, @CreatedAt);
        """;

            var parameters = new
            {
                Id = Guid.NewGuid(),
                Entity = entity,
                EntityId = entityId,
                Action = action,
                Data = data,
                CreatedAt = DateTime.UtcNow
            };

            using var connection = connectionFactory.Create();
            Console.WriteLine("CONNECTION STRING DO AUDIT:");
            Console.WriteLine(connection.ConnectionString);

            await connection.ExecuteAsync(
                new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));
        }
    }
}
