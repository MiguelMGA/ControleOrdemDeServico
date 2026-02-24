using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetAllCustomers
{
    public sealed class GetAllCustomersHandler
    : IRequestHandler<GetAllCustomersQuery, IEnumerable<CustomerEntity>>
    {
        private readonly ICustomerRepository _repo;

        public GetAllCustomersHandler(ICustomerRepository repo)
            => _repo = repo;

        public async Task<IEnumerable<CustomerEntity>> Handle(
            GetAllCustomersQuery request,
            CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync(cancellationToken);
        }
    }
}
