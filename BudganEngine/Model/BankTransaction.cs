using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Model;

[ExcludeFromCodeCoverage]
public class BankTransaction
{
    public string Key { get; init; }
    
    public BankTransactionSource Source { get; init; }
    public string LayoutName { get; init; }
    public DateOnly DateTransaction { get; init; }
    
    public DateOnly DateInscription { get; init; }
    
    public string Amount { get; init; }
    
    public string Description { get; init; }
    
    public string CardNumber { get; init; }
}
