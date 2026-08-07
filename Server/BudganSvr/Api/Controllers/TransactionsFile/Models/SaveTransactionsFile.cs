namespace BudganSvr.Api.Controllers.TransactionsFile.Models;

public class SaveTransactionsFile
{
    public required Guid AccountId { get; set; }
    public required string Content { get; set; }
    public required string Filename { get; set; }
    public required DateOnly InsertionDate { get; set; }
}
