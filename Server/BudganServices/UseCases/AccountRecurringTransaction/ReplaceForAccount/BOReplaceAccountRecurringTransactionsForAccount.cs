namespace BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;

public class BOReplaceAccountRecurringTransactionsForAccount
{
    public required Guid AccountId { get; set; }
    public required List<BOAccountRecurringTransactionItem> Items { get; set; }
}
