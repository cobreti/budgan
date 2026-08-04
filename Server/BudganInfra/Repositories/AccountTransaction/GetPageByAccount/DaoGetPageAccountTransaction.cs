namespace BudganInfra.Repositories.AccountTransaction.GetPageByAccount;

public class DaoGetPageAccountTransaction
{
    public required List<DaoPageItemAccountTransaction> Items { get; set; }
    public required int TotalCount { get; set; }
}
