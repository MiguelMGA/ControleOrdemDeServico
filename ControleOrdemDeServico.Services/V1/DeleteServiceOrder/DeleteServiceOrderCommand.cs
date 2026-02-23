using MediatR;

namespace OsService.Services.V1.DeleteServiceOrder
{
    public sealed record DeleteServiceOrderCommand(Guid Id) : IRequest<Unit>;
}