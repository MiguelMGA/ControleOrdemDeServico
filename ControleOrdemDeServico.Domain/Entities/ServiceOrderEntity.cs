using OsService.Domain.Enums;
using OsService.Domain.Exceptions;
using OsService.Domain.ValueObjects;

namespace OsService.Domain.Entities;

public sealed class ServiceOrderEntity
{
    public Guid Id { get; }
    public int Number { get; internal set; }
    public Guid CustomerId { get; }
    public string Description { get; private set; }
    public ServiceOrderStatus Status { get; private set; }

    public DateTime OpenedAt { get; internal set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? FinishedAt { get; private set; }

    public decimal? Price { get; private set; }
    public Currency Currency { get; private set; }
    public DateTime? UpdatedPriceAt { get; private set; }

    public bool IsDeletedFlag { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    private ServiceOrderEntity(
        Guid id,
        Guid customerId,
        string description,
        DateTime openedAt)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("CustomerId não pode estar vazio.");

        ValidateDescription(description);

        Id = id;
        CustomerId = customerId;
        Description = description.Trim();

        Status = ServiceOrderStatus.Open;
        OpenedAt = openedAt;

        Currency = Currency.Create("BRL");
    }

    public static ServiceOrderEntity Create(
        Guid customerId,
        string description)
    {
        return new ServiceOrderEntity(
            Guid.NewGuid(),
            customerId,
            description,
            DateTime.UtcNow);
    }

    public static ServiceOrderEntity Restore(Snapshot snapshot)
    {
        var entity = new ServiceOrderEntity(
            snapshot.Id,
            snapshot.CustomerId,
            snapshot.Description,
            snapshot.OpenedAt);

        entity.Number = snapshot.Number;
        entity.Status = snapshot.Status;
        entity.StartedAt = snapshot.StartedAt;
        entity.FinishedAt = snapshot.FinishedAt;
        entity.Price = snapshot.Price;
        entity.Currency = Currency.Create(snapshot.CurrencyCode);
        entity.UpdatedPriceAt = snapshot.UpdatedPriceAt;

        entity.IsDeletedFlag = snapshot.IsDeleted;
        entity.DeletedAt = snapshot.DeletedAt;

        return entity;
    }

    internal void SetNumber(int number)
        => Number = number;

    public void Start()
    {
        ChangeStatus(ServiceOrderStatus.InProgress);
        StartedAt = DateTime.UtcNow;
    }

    public void Finish()
    {
        if (Price is null)
            throw new DomainException(
                "Não é possível finalizar a ordem de serviço sem o valor.");

        ChangeStatus(ServiceOrderStatus.Finished);
        FinishedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("O valor não pode ser negativo.");

        if (Status.IsFinalState())
            throw new DomainException(
                "Não é possível alterar o valor após a finalização.");

        Price = price;
        Currency = Currency.Create("BRL");
        UpdatedPriceAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        if (IsDeletedFlag) return;

        IsDeletedFlag = true;
        DeletedAt = DateTime.UtcNow;
    }

    public bool IsDeleted() => IsDeletedFlag;

    private void ChangeStatus(ServiceOrderStatus newStatus)
    {
        if (!Status.CanTransitionTo(newStatus))
            throw new DomainException(
                $"Transição inválida de {Status} para {newStatus}.");

        Status = newStatus;
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição é obrigatória.");

        description = description.Trim();

        if (description.Length is < 1 or > 500)
            throw new DomainException(
                "A descrição deve ter entre 1 e 500 caracteres.");
    }

    public sealed record Snapshot(
        Guid Id,
        int Number,
        Guid CustomerId,
        string Description,
        ServiceOrderStatus Status,
        DateTime OpenedAt,
        DateTime? StartedAt,
        DateTime? FinishedAt,
        decimal? Price,
        string CurrencyCode,
        DateTime? UpdatedPriceAt,
        bool IsDeleted,
        DateTime? DeletedAt);
}