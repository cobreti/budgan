namespace BudganServices.UseCases.TransactionsFile.GetList;

public class BOTransactionsFile
{
    public required Guid Id { get; set; }
    public required Guid AccountId { get; set; }
    public required string Content { get; set; }
    public required string Filename { get; set; }
    public required DateOnly InsertionDate { get; set; }
}
