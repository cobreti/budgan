namespace BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;

public class BOAccountRecurringTransactionItem
{
    public required string RecurringId { get; set; }
    public required double PeriodInDays { get; set; }
    public required int TransactionCount { get; set; }
    public required string Description { get; set; }
    public required decimal AverageAmount { get; set; }
    public required string FirstOccurrenceDateAsString { get; set; }
    public required string LastOccurrenceDateAsString { get; set; }
}
