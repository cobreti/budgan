namespace BudganServices.UseCases.AccountTransaction.Create;

public class BOCreateAccountTransaction
{
    public required Guid AccountId { get; set; }
    public Guid? FileId { get; set; }
    public required string CardNumber { get; set; }
    public required string DateInscriptionAsString { get; set; }
    public required decimal Amount { get; set; }
    public required string Description { get; set; }
}
