using MediatR;
using OsService.Domain.Entities;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.OpenServiceOrder;

public sealed class OpenServiceOrderHandler(
    ICustomerRepository customers,
    IServiceOrderRepository serviceOrders
) : IRequestHandler<OpenServiceOrderCommand, (Guid Id, int Number)>
{
    public async Task<(Guid Id, int Number)> Handle(OpenServiceOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
            throw new ArgumentException("CustomerId é obrigatório.");

        var exists = await customers.ExistsAsync(request.CustomerId, cancellationToken);
        if (!exists)
            throw new KeyNotFoundException("Customer não encontrado.");

        var serviceOrder = ServiceOrderEntity.Create(
            request.CustomerId,
            request.Description);

        await serviceOrders.InsertAsync(serviceOrder, cancellationToken);

        return (serviceOrder.Id, serviceOrder.Number);
    }
}
