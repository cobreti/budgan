namespace BudganServices.UseCases.AccountTransaction.SetSnapshot;

public class BOSetAccountTransactionSnapshot
{
    public required Guid AccountId { get; set; }
    public required string DateAsString { get; set; }
    public required decimal Amount { get; set; }
}
