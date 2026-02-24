using MediatR;
using OsService.Domain.Entities;

namespace OsService.Services.V1.GetAllServiceOrders
{
    public sealed record GetAllServiceOrdersQuery(Guid? CustomerId)
    : IRequest<IEnumerable<ServiceOrderEntity>>;
}
