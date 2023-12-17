namespace Budgan.Model;

public class BankTransaction
{
    public string Key { get; init; }
    public string LayoutName { get; init; }
    public string Origin { get; init; }
    
    public DateOnly DateTransaction { get; init; }
    
    public DateOnly DateInscription { get; init; }
    
    public string Amount { get; init; }
    
    public string Description { get; init; }
    
    public string CardNumber { get; init; }
}
