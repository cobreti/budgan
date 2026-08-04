namespace BudganInfra.Repositories.AccountTransaction.UpdateBalances;

public class DaoAccountTransactionBalanceUpdate
{
    public required Guid Id { get; set; }
    public decimal? Balance { get; set; }
    public int? BalanceDateOffset { get; set; }
}
