using MediatR;

namespace OsService.Services.V1.GetServiceOrderById
{
    public sealed record GetServiceOrderByIdQuery(Guid Id)
    : IRequest<ServiceOrderResponse?>;
}
