using System.Text.RegularExpressions;

namespace OsService.Domain.Shared;

internal static partial class EmailRegex
{
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    public static partial Regex Instance();
}