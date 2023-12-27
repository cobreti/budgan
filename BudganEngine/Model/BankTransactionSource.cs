using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Model;

[ExcludeFromCodeCoverage]
public record BankTransactionSource
{
    public required string InputId { get; init; }
    public required string FileRelativePath { get; init; }
}
