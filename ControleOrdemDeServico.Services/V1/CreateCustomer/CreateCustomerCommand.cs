using MediatR;

namespace OsService.Services.V1.CreateCustomer;

/// <summary>
/// Comando responsável por criar um novo cliente no sistema.
/// </summary>
/// <param name="Name">Nome completo do cliente.</param>
/// <param name="Phone">Telefone para contato do cliente.</param>
/// <param name="Email">Endereço de e-mail do cliente.</param>
/// <param name="Document">Documento de identificação (CPF ou CNPJ).</param>
public sealed record CreateCustomerCommand(
    string Name,
    string? Phone,
    string? Email,
    string? Document
) : IRequest<Guid>;