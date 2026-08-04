using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.DeleteByAccount;

public class DeleteAccountRecurringTransactionByAccountRepoOp : BaseRepositoryOperation, IDeleteAccountRecurringTransactionByAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public DeleteAccountRecurringTransactionByAccountRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var entities = await this._dataContext.AccountRecurringTransactions
            .Where(x => x.AccountId == this._accountId)
            .ToListAsync();

        if (entities.Count > 0)
        {
            this._dataContext.AccountRecurringTransactions.RemoveRange(entities);
            await this._dataContext.SaveChangesAsync();
        }

        this.SetSucceeded();
    }
}
