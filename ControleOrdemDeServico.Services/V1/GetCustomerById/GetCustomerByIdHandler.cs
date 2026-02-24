using MediatR;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetCustomerById
{
    public sealed class GetCustomerByIdHandler(
    ICustomerRepository repository)
    : IRequestHandler<GetCustomerByIdQuery, CustomerResponse?>
    {
        public async Task<CustomerResponse?> Handle(
            GetCustomerByIdQuery request,
            CancellationToken cancellationToken)
        {
            var customer = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (customer is null)
                throw new NotFoundException("Cliente não encontrado.");

            return new CustomerResponse(
                customer.Id,
                customer.Name,
                customer.Document,
                customer.Email,
                customer.Phone,
                customer.CreatedAt);
        }
    }
}
