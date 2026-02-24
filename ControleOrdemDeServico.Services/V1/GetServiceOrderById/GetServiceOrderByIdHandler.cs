using MediatR;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Repository;

namespace OsService.Services.V1.GetServiceOrderById
{
    public sealed class GetServiceOrderByIdHandler(
        IServiceOrderRepository repository)
        : IRequestHandler<GetServiceOrderByIdQuery, ServiceOrderResponse?>
    {
        public async Task<ServiceOrderResponse?> Handle(
            GetServiceOrderByIdQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await repository.GetByIdAsync(request.Id, cancellationToken);

            if (entity is null)
                throw new NotFoundException("Ordem de Serviço não encontrada.");

            return new ServiceOrderResponse(
                entity.Id,
                entity.Number,
                entity.CustomerId,
                entity.Description,
                entity.Status.ToString(),
                entity.OpenedAt,
                entity.StartedAt,
                entity.FinishedAt,
                entity.Price,
                entity.Currency.Code,
                entity.UpdatedPriceAt,
                entity.IsDeleted(),
                entity.DeletedAt);
        }
    }
}