using OsService.Domain.Exceptions;

namespace OsService.Domain.ValueObjects;

public sealed class Currency
{
    public string Code { get; }

    private Currency(string code)
    {
        Code = code;
    }

    public static Currency Create(string code)
    {
        if (!string.Equals(code, "BRL", StringComparison.OrdinalIgnoreCase))
            throw new DomainException("A única moeda suportada é BRL.");

        return new Currency("BRL");
    }

    public override string ToString() => Code;
}