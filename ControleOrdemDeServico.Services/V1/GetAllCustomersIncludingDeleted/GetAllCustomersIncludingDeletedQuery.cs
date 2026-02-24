using MediatR;
using OsService.Domain.Entities;

namespace OsService.Services.V1.GetAllCustomersIncludingDeleted
{
    public sealed record GetAllCustomersIncludingDeletedQuery()
        : IRequest<IEnumerable<CustomerEntity>>;
}
