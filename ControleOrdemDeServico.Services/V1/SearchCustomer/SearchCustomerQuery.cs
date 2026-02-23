using MediatR;
using OsService.Domain.Entities;

namespace OsService.Services.V1.SearchCustomer;

public sealed record SearchCustomerQuery(string? Document, string? Phone) : IRequest<IEnumerable<CustomerEntity>>;