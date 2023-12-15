namespace Budgan.Model;

public class BankTransactionsLayout
{
    public string Name { get; init; }
    public int? DateTransaction { get; init; }
    
    public int? DateInscription { get; init; }
    
    public int? Amount { get; init; }
    
    public int? Description { get; init; }
    
    public int? CardNumber { get; init; }
    
    public string[]? Key { get; init; }
    
    public int? MinColumnsRequired { get; set; }

    public int? GetIndexByName(string name)
    {
        switch (name)
        {
            case "DateTransaction":
                return DateTransaction;
            case "DateInscription":
                return DateInscription;
            case "Amount":
                return Amount;
            case "Description":
                return Description;
            case "CardNumber":
                return CardNumber;
            
            default:
                return null;
        }
    }
}
