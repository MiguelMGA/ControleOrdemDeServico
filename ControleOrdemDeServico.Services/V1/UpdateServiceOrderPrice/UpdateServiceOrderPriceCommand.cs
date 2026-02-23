using MediatR;

namespace OsService.Services.V1.UpdateServiceOrderPrice;

public sealed record UpdateServiceOrderPriceCommand(
    Guid Id,
    decimal Price
) : IRequest<Unit>;