using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudganInfra.DBContext.Tables;

[Table("AccountRecurringTransactions")]
public class AccountRecurringTransaction : BaseEntity
{
    [MaxLength(450)]
    public required string RecurringId { get; set; }

    public required Guid AccountId { get; set; }
    public Account Account { get; set; }

    public required double PeriodInDays { get; set; }
    public required int TransactionCount { get; set; }

    [MaxLength(450)]
    public required string Description { get; set; }

    public required decimal AverageAmount { get; set; }
    public required DateOnly FirstOccurrenceDate { get; set; }
    public required DateOnly LastOccurrenceDate { get; set; }
}
