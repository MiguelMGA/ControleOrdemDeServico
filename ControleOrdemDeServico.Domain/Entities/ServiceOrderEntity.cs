using OsService.Domain.Enums;
using OsService.Domain.Exceptions;
using OsService.Domain.ValueObjects;

namespace OsService.Domain.Entities;

public sealed class ServiceOrderEntity
{
    public Guid Id { get; }
    public int Number { get; private set; }
    public Guid CustomerId { get; }
    public string Description { get; private set; }
    public ServiceOrderStatus Status { get; private set; }
    public DateTime OpenedAt { get; }
    public decimal? Price { get; private set; }
    public Currency Currency { get; private set; }
    public DateTime? UpdatedPriceAt { get; private set; }

    private ServiceOrderEntity(
        Guid id,
        int number,
        Guid customerId,
        string description)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId não pode estar vazio.");

        ValidateDescription(description);

        Id = id;
        Number = number;
        CustomerId = customerId;
        Description = description.Trim();
        Status = ServiceOrderStatus.Open;
        OpenedAt = DateTime.UtcNow;
        Currency = Currency.Create("BRL");
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição é obrigatória.");

        if (description.Length > 500)
            throw new DomainException("A descrição deve ter no máximo 500 caracteres.");
    }

    public static ServiceOrderEntity Create(
        int number,
        Guid customerId,
        string description)
    {
        return new ServiceOrderEntity(
            Guid.NewGuid(),
            number,
            customerId,
            description);
    }

    public void Start()
    {
        ChangeStatus(ServiceOrderStatus.InProgress);
    }

    public void Finish()
    {
        if (Price is null)
            throw new DomainException("Não é possível encerrar a ordem de serviço sem o preço.");

        ChangeStatus(ServiceOrderStatus.Finished);
    }

    public void Cancel()
    {
        ChangeStatus(ServiceOrderStatus.Canceled);
    }

    public void UpdatePrice(decimal price, string currencyCode)
    {
        if (price < 0)
            throw new DomainException("O valor não pode ser negativo.");

        if (Status is ServiceOrderStatus.Finished or ServiceOrderStatus.Canceled)
            throw new DomainException("Não é possível alterar o preço após a ordem de serviço ser finalizada ou cancelada.");

        Price = price;
        Currency = Currency.Create(currencyCode);
        UpdatedPriceAt = DateTime.UtcNow;
    }

    private void ChangeStatus(ServiceOrderStatus newStatus)
    {
        if (!Status.CanTransitionTo(newStatus))
            throw new DomainException(
                $"Transição de status inválida de {Status} para {newStatus}.");

        Status = newStatus;
    }
}
