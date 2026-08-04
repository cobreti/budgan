namespace BudganInfra.Repositories.AccountRecurringTransaction.GetList;

public class DaoListAccountRecurringTransaction
{
    public required Guid Id { get; set; }
    public required Guid AccountId { get; set; }
    public required string RecurringId { get; set; }
    public required double PeriodInDays { get; set; }
    public required int TransactionCount { get; set; }
    public required string Description { get; set; }
    public required decimal AverageAmount { get; set; }
    public required DateOnly FirstOccurrenceDate { get; set; }
    public required DateOnly LastOccurrenceDate { get; set; }
}
