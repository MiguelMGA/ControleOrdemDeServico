using MediatR;
using OsService.Domain.Enums;

namespace OsService.Services.V1.UpdateServiceOrderStatus;

public sealed record UpdateServiceOrderStatusCommand(
    Guid Id,
    ServiceOrderStatus NewStatus
) : IRequest<Unit>;