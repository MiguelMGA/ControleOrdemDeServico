using MediatR;

namespace OsService.Services.V1.DeleteCustomer
{
    public sealed record DeleteCustomerCommand(Guid Id) : IRequest<Unit>;
}