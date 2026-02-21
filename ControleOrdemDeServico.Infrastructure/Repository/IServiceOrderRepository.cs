using OsService.Domain.Entities;

namespace OsService.Infrastructure.Repository;

public interface IServiceOrderRepository
{
    Task<int> GetNextNumberAsync(CancellationToken ct);
    Task InsertAsync(ServiceOrderEntity entity, CancellationToken ct);
    Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct);
}
