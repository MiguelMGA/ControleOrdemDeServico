using OsService.Domain.Exceptions;
using OsService.Domain.Shared;

namespace OsService.Domain.Entities;

public sealed class CustomerEntity
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Document { get; private set; }
    public DateTime CreatedAt { get; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private CustomerEntity(CustomerSnapshot snapshot)
    {
        Id = snapshot.Id;
        Name = string.Empty;
        SetName(snapshot.Name);
        SetPhone(snapshot.Phone);
        SetEmail(snapshot.Email);
        SetDocument(snapshot.Document);

        CreatedAt = snapshot.CreatedAt;
        IsDeleted = snapshot.IsDeleted;
        DeletedAt = snapshot.DeletedAt;
    }
    public static CustomerEntity Create(
    string name,
    string? phone,
    string? email,
    string? document)
    {
        var snapshot = new CustomerSnapshot
        {
            Id = Guid.NewGuid(),
            Name = name,
            Phone = phone,
            Email = email,
            Document = document,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            DeletedAt = null
        };

        return new CustomerEntity(snapshot);
    }

    public static CustomerEntity Restore(CustomerSnapshot snapshot)
    {
        return new CustomerEntity(snapshot);
    }

    public void UpdateName(string name) => SetName(name);
    public void UpdatePhone(string? phone) => SetPhone(phone);
    public void UpdateEmail(string? email) => SetEmail(email);
    public void UpdateDocument(string? document) => SetDocument(document);

    public void MarkAsDeleted()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted(DateTime? deletedAt)
    {
        IsDeleted = true;
        DeletedAt = deletedAt ?? DateTime.UtcNow;
    }

    public bool IsDeletedCustomer() => IsDeleted;

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do cliente é obrigatório.");

        name = name.Trim();

        if (name.Length < 2 || name.Length > 150)
            throw new DomainException("Nome deve ter entre 2 e 150 caracteres.");

        Name = name;
    }

    private void SetPhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            Phone = null;
            return;
        }

        phone = phone.Trim();

        if (phone.Length > 30)
            throw new DomainException("Telefone deve ter no máximo 30 caracteres.");

        Phone = phone;
    }

    private void SetEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            Email = null;
            return;
        }

        email = email.Trim();

        if (email.Length > 120)
            throw new DomainException("E-mail deve ter no máximo 120 caracteres.");

        if (!EmailRegex.Instance().IsMatch(email))
            throw new DomainException("Formato de e-mail inválido.");

        Email = email;
    }

    private void SetDocument(string? document)
    {
        if (string.IsNullOrWhiteSpace(document))
        {
            Document = null;
            return;
        }

        document = document.Trim();

        if (document.Length > 30)
            throw new DomainException("Documento deve ter no máximo 30 caracteres.");

        Document = document;
    }
}

public sealed class CustomerSnapshot
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Document { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? DeletedAt { get; init; }
}