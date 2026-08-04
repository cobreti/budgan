namespace BudganSvr.Api.Controllers.AccountTransaction.Models;

public class SetAccountTransactionSnapshot
{
    public required string DateAsString { get; set; }
    public required decimal Amount { get; set; }
}
