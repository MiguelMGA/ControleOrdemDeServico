using MediatR;

namespace OsService.Services.V1.GetCustomerById
{
    public sealed record GetCustomerByIdQuery(Guid Id)
    : IRequest<CustomerResponse?>;
}
