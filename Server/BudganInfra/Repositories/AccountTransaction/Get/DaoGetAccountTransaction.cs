using BudganInfra.DBContext.Tables;
using BudganInfra.Repositories.Models;

namespace BudganInfra.Repositories.AccountTransaction.Get;

public class DaoGetAccountTransaction : DaoBaseUpdateModel
{
    public required Guid AccountId { get; set; }
    public required string UniqueKey { get; set; }
    public required string RecurringId { get; set; }
    public Guid? FileId { get; set; }
    public required string CardNumber { get; set; }
    public required DateOnly DateInscription { get; set; }
    public required decimal Amount { get; set; }
    public decimal? Balance { get; set; }
    public int? BalanceDateOffset { get; set; }
    public required string Description { get; set; }
    public required AccountTransactionRecordType RecordType { get; set; }
}
