using OsService.Domain.Exceptions;

namespace OsService.Domain.ValueObjects;

public sealed class Currency
{
    private static readonly HashSet<string> Allowed =
    [
        "BRL",
        "USD",
        "EUR"
    ];

    public string Code { get; }

    private Currency(string code)
    {
        Code = code;
    }

    public static Currency Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Currency cannot be empty.");

        code = code.ToUpperInvariant();

        if (!Allowed.Contains(code))
            throw new DomainException($"Currency '{code}' is not supported.");

        return new Currency(code);
    }

    public override string ToString() => Code;
}
