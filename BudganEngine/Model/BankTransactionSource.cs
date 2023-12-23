namespace BudganEngine.Model;

public record BankTransactionSource
{
    public required string InputId { get; init; }
    public required string FileRelativePath { get; init; }
}
