using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudganInfra.DBContext.Tables;

public enum AccountTransactionRecordType
{
    Normal,
    Snapshot
}

[Table("AccountTransactions")]
public class AccountTransaction : BaseEntity
{
    [MaxLength(450)]
    public required string UniqueKey { get; set; }

    [MaxLength(450)]
    public required string RecurringId { get; set; }

    public Guid? FileId { get; set; }

    public required Guid AccountId { get; set; }
    public Account Account { get; set; }

    [MaxLength(50)]
    public required string CardNumber { get; set; }

    public required DateOnly DateInscription { get; set; }

    public required decimal Amount { get; set; }

    public decimal? Balance { get; set; }

    public int? BalanceDateOffset { get; set; }

    [MaxLength(450)]
    public required string Description { get; set; }

    public required AccountTransactionRecordType RecordType { get; set; }
}
