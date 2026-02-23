using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.SearchCustomer;

public sealed class SearchCustomerHandler : IRequestHandler<SearchCustomerQuery, IEnumerable<CustomerEntity>>
{
    private readonly ICustomerRepository _repo;

    public SearchCustomerHandler(ICustomerRepository repo) => _repo = repo;

    public async Task<IEnumerable<CustomerEntity>> Handle(SearchCustomerQuery request, CancellationToken cancellationToken)
    {
        var document = request.Document?.Trim();
        var phone = request.Phone?.Trim();

        return await _repo.SearchAsync(document, phone, cancellationToken);
    }
}