namespace BudganSvr.Api.Controllers.AccountTransaction.Models;

public class GetPageAccountTransaction
{
    public required List<AccountTransactionPageItem> Items { get; set; }
    public required int TotalCount { get; set; }
}
