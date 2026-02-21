using OsService.Domain.Entities;

namespace OsService.Infrastructure.Repository;

public interface IServiceOrderRepository
{
    Task InsertAsync(ServiceOrderEntity entity, CancellationToken ct);
    Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct);
}
