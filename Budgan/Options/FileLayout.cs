namespace Budgan.Options;

public class FileLayout
{
    public int? DateTransaction { get; set; }
    
    public int? DateInscription { get; set; }
    
    public int? Amount { get; set; }
    
    public int? Description { get; set; }
    
    public int? CardNumber { get; set; }
    
    public string[]? Key { get; set; }

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
