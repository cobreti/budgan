namespace BudganSvr.Api.Controllers.AccountRecurringTransaction.Models;

public class AccountRecurringTransactionListItem
{
    public required string Id { get; set; }
    public required Guid AccountId { get; set; }
    public required double PeriodInDays { get; set; }
    public required int TransactionCount { get; set; }
    public required string Description { get; set; }
    public required decimal AverageAmount { get; set; }
    public required string FirstOccurrenceDate { get; set; }
    public required string LastOccurrenceDate { get; set; }
}
