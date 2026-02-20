using OsService.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace OsService.Domain.Entities
{
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
        string? document)
        {
            Id = id;

            Name = string.Empty;
            SetName(name);

            SetPhone(phone);
            SetEmail(email);
            SetDocument(document);
            CreatedAt = DateTime.UtcNow;
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
                document);
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

            Name = name.Trim();
        }

        private void SetPhone(string? phone)
        {
            Phone = string.IsNullOrWhiteSpace(phone)
                ? null
                : phone.Trim();
        }

        private void SetEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Email = null;
                return;
            }

            var isValid = Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            if (!isValid)
                throw new DomainException("Formato de e-mail inválido.");

            Email = email.Trim();
        }

        private void SetDocument(string? document)
        {
            if (string.IsNullOrWhiteSpace(document))
            {
                Document = null;
                return;
            }

            Document = document.Trim();
        }
    }
}
