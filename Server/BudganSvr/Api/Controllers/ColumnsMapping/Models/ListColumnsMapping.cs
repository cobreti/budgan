namespace BudganSvr.Api.Controllers.ColumnsMapping.Models;

public class ListColumnsMapping
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    
    public required int CardNumberColumnIndex { get; set; }
    public string? CardNumberColumnText { get; set; }
    
    public required int DateInscriptionColumnIndex { get; set; }
    public string? DateInscriptionColumnText { get; set; }
    
    public required int AmountColumnIndex { get; set; }
    public string? AmountColumnText { get; set; }
    
    public required int DescriptionColumnIndex { get; set; }
    public string? DescriptionColumnText { get; set; }
}