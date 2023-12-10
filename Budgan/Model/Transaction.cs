namespace Budgan.Model;

public class Transaction
{
    public string Key { get; set; }
    public string LayoutName { get; set; }
    public string Origin { get; set; }
    
    public string DateTransaction { get; set; }
    
    public string DateInscription { get; set; }
    
    public string Amount { get; set; }
    
    public string Description { get; set; }
    
    public string CardNumber { get; set; }
}
