namespace BudganServices.UseCases.AccountTransaction.GetPageByAccount;

public class BOGetPageAccountTransaction
{
    public required List<BOAccountTransactionPageItem> Items { get; set; }
    public required int TotalCount { get; set; }
}
