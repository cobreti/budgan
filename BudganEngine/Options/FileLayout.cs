using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Options;

[ExcludeFromCodeCoverage]
public class FileLayout
{
    public int? MinColumnsRequired { get; set; } 
    public int? DateTransaction { get; set; }
    
    public int? DateInscription { get; set; }
    
    public int? Amount { get; set; }
    
    public int? Description { get; set; }
    
    public int? CardNumber { get; set; }
    
    public string[]? Key { get; set; }
}
