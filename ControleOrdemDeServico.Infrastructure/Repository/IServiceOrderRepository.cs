using OsService.Domain.Entities;

namespace OsService.Infrastructure.Repository;

public interface IServiceOrderRepository
{
    Task InsertAsync(ServiceOrderEntity entity, CancellationToken ct);
    Task<ServiceOrderEntity?> GetByIdAsync(Guid id, CancellationToken ct);
    Task UpdateAsync(ServiceOrderEntity entity, CancellationToken ct);
    Task<IEnumerable<ServiceOrderEntity>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct);

    Task<IEnumerable<ServiceOrderEntity>> GetAllAsync(
    Guid? customerId,
    CancellationToken ct);

    Task<IEnumerable<ServiceOrderEntity>> GetAllIncludingDeletedAsync(
        Guid? customerId,
        CancellationToken ct);
}
