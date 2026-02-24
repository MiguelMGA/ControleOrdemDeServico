using MediatR;
using OsService.Domain.Entities;

namespace OsService.Services.V1.GetAllCustomers
{
    public sealed record GetAllCustomersQuery()
    : IRequest<IEnumerable<CustomerEntity>>;
}
