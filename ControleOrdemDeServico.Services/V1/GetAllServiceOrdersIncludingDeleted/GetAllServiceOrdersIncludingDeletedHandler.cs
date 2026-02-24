using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetAllServiceOrdersIncludingDeleted
{
    public sealed class GetAllServiceOrdersIncludingDeletedHandler
    : IRequestHandler<GetAllServiceOrdersIncludingDeletedQuery, IEnumerable<ServiceOrderEntity>>
    {
        private readonly IServiceOrderRepository _repo;

        public GetAllServiceOrdersIncludingDeletedHandler(IServiceOrderRepository repo)
            => _repo = repo;

        public async Task<IEnumerable<ServiceOrderEntity>> Handle(
            GetAllServiceOrdersIncludingDeletedQuery request,
            CancellationToken cancellationToken)
        {
            return await _repo.GetAllIncludingDeletedAsync(request.CustomerId, cancellationToken);
        }
    }
}
