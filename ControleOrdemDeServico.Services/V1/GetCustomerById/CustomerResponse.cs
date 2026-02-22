namespace OsService.Services.V1.GetCustomerById
{
    public sealed record CustomerResponse(
    Guid Id,
    string Name,
    string? Document,
    string? Email,
    string? Phone,
    DateTime CreatedAt);
}
