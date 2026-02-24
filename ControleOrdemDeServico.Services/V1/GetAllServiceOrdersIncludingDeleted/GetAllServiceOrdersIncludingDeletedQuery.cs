using MediatR;
using OsService.Domain.Entities;

namespace OsService.Services.V1.GetAllServiceOrdersIncludingDeleted
{
    public sealed record GetAllServiceOrdersIncludingDeletedQuery(Guid? CustomerId)
    : IRequest<IEnumerable<ServiceOrderEntity>>;
}
