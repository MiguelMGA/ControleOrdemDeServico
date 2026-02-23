namespace OsService.Infrastructure.Logging
{
    public interface IAuditLogRepository
    {
        Task AddAsync(
            string entity,
            Guid? entityId,
            string action,
            string? data,
            CancellationToken cancellationToken);
    }
}
