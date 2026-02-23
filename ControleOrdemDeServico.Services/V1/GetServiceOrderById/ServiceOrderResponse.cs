namespace OsService.Services.V1.GetServiceOrderById
{
    public sealed record ServiceOrderResponse(
        Guid Id,
        int Number,
        Guid CustomerId,
        string Description,
        string Status,
        DateTime OpenedAt,
        DateTime? StartedAt,
        DateTime? FinishedAt,
        decimal? Price,
        string Currency,
        DateTime? UpdatedPriceAt,
        bool IsDeleted,
        DateTime? DeletedAt);
}