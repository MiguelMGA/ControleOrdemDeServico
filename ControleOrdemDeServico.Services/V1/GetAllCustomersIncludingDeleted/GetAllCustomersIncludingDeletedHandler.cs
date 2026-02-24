using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetAllCustomersIncludingDeleted
{
    public sealed class GetAllCustomersIncludingDeletedHandler
    : IRequestHandler<GetAllCustomersIncludingDeletedQuery, IEnumerable<CustomerEntity>>
    {
        private readonly ICustomerRepository _repo;

        public GetAllCustomersIncludingDeletedHandler(ICustomerRepository repo)
            => _repo = repo;

        public async Task<IEnumerable<CustomerEntity>> Handle(
            GetAllCustomersIncludingDeletedQuery request,
            CancellationToken cancellationToken)
        {
            return await _repo.GetAllIncludingDeletedAsync(cancellationToken);
        }
    }
}
