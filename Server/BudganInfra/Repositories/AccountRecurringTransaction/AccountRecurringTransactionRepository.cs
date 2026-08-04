using BudganInfra.DBContext;
using BudganInfra.Repositories.AccountRecurringTransaction.DeleteByAccount;
using BudganInfra.Repositories.AccountRecurringTransaction.GetList;
using BudganInfra.Repositories.AccountRecurringTransaction.GetListByAccount;
using BudganInfra.Repositories.AccountRecurringTransaction.Insert;
using BudganInfra.Repositories.AccountRecurringTransaction.ReplaceForAccount;
using Microsoft.Extensions.Logging;

namespace BudganInfra.Repositories.AccountRecurringTransaction;

internal class AccountRecurringTransactionRepository : IAccountRecurringTransactionRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<AccountRecurringTransactionRepository> _logger;

    public AccountRecurringTransactionRepository(ILogger<AccountRecurringTransactionRepository> logger, DataContext dataContext)
    {
        this._logger = logger;
        this._dataContext = dataContext;
    }

    public IInsertAccountRecurringTransactionRepoOp InsertAccountRecurringTransactionRepoOperation(
        List<DaoInsertAccountRecurringTransaction> items)
    {
        return new InsertAccountRecurringTransactionRepoOp(this._dataContext, items);
    }

    public IListAccountRecurringTransactionRepoOp ListAccountRecurringTransactionRepoOperation()
    {
        return new ListAccountRecurringTransactionRepoOp(this._dataContext);
    }

    public IListAccountRecurringTransactionByAccountRepoOp ListAccountRecurringTransactionByAccountRepoOperation(Guid accountId)
    {
        return new ListAccountRecurringTransactionByAccountRepoOp(this._dataContext, accountId);
    }

    public IDeleteAccountRecurringTransactionByAccountRepoOp DeleteAccountRecurringTransactionByAccountRepoOperation(Guid accountId)
    {
        return new DeleteAccountRecurringTransactionByAccountRepoOp(this._dataContext, accountId);
    }

    public IReplaceAccountRecurringTransactionsForAccountRepoOp ReplaceAccountRecurringTransactionsForAccountRepoOperation(
        Guid accountId,
        List<DaoInsertAccountRecurringTransaction> items)
    {
        return new ReplaceAccountRecurringTransactionsForAccountRepoOp(this._dataContext, accountId, items);
    }
}
