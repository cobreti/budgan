namespace BudganServices.UseCases.TransactionsFile;

public class BOSaveTransactionsFile
{
    public required Guid AccountId { get; set; }
    public required string Content { get; set; }
    public required string Filename { get; set; }
    public required DateOnly InsertionDate { get; set; }
}
