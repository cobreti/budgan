namespace BudganEngine.Model;

public class BankTransactionsLayout
{
    public static Dictionary<string, Func<BankTransactionsLayout, int?>> IndexByNameMapping { get; } = new();

    public static void AddColumnNameMapping(string columnName, Func<BankTransactionsLayout, int?> mappingFct)
    {
        var name = columnName.ToLowerInvariant().Replace("index", "");
        IndexByNameMapping.TryAdd(name, mappingFct);
    }

    public static int? GetIndexMappingByName(BankTransactionsLayout layout, string columnName)
    {
        var name = columnName.ToLowerInvariant().Replace("index", "");
        if (IndexByNameMapping.TryGetValue(name, out var fct))
        {
            return fct(layout);
        }

        throw new ArgumentException($"Invalid column name : {columnName}");
    }

    static BankTransactionsLayout()
    {
        AddColumnNameMapping("DateTransaction", layout => layout.DateTransaction);
        AddColumnNameMapping("DateInscription", layout => layout.DateInscription);
        AddColumnNameMapping("Amount", layout => layout.Amount);
        AddColumnNameMapping("Description", layout => layout.Description);
        AddColumnNameMapping("CardNumber", layout => layout.CardNumber);
    }
    
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
        return GetIndexMappingByName(this, name);
    }
}
