using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetAllServiceOrders
{
    public sealed class GetAllServiceOrdersHandler
    : IRequestHandler<GetAllServiceOrdersQuery, IEnumerable<ServiceOrderEntity>>
    {
        private readonly IServiceOrderRepository _repo;

        public GetAllServiceOrdersHandler(IServiceOrderRepository repo)
            => _repo = repo;

        public async Task<IEnumerable<ServiceOrderEntity>> Handle(
            GetAllServiceOrdersQuery request,
            CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(request.CustomerId, cancellationToken);
        }
    }
}
