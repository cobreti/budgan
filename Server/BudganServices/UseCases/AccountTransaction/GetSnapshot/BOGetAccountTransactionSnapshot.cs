using BudganInfra.DBContext.Tables;

namespace BudganServices.UseCases.AccountTransaction.GetSnapshot;

public class BOGetAccountTransactionSnapshot
{
    public required string Id { get; set; }
    public required DateTime Timestamp { get; set; }
    public required Guid AccountId { get; set; }
    public Guid? FileId { get; set; }
    public required string CardNumber { get; set; }
    public required string DateInscriptionAsString { get; set; }
    public required decimal Amount { get; set; }
    public decimal? Balance { get; set; }
    public int? BalanceDateOffset { get; set; }
    public required string Description { get; set; }
    public required string UniqueKey { get; set; }
    public required string RecurringId { get; set; }
    public required AccountTransactionRecordType RecordType { get; set; }
}
