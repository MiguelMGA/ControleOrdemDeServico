using OsService.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace OsService.Domain.Entities;

public sealed class CustomerEntity
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public string? Document { get; private set; }
    public DateTime CreatedAt { get; }

    private CustomerEntity(
        Guid id,
        string name,
        string? phone,
        string? email,
        string? document,
        DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;

        Name = string.Empty;
        SetName(name);
        SetPhone(phone);
        SetEmail(email);
        SetDocument(document);
    }

    public static CustomerEntity Create(
        string name,
        string? phone,
        string? email,
        string? document)
    {
        return new CustomerEntity(
            Guid.NewGuid(),
            name,
            phone,
            email,
            document,
            DateTime.UtcNow);
    }

    public static CustomerEntity Restore(
        Guid id,
        string name,
        string? phone,
        string? email,
        string? document,
        DateTime createdAt)
    {
        return new CustomerEntity(
            id,
            name,
            phone,
            email,
            document,
            createdAt);
    }

    public void UpdateName(string name)
        => SetName(name);

    public void UpdatePhone(string? phone)
        => SetPhone(phone);

    public void UpdateEmail(string? email)
        => SetEmail(email);

    public void UpdateDocument(string? document)
        => SetDocument(document);

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

        var isValid = Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        if (!isValid)
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