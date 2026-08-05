using BudganInfra.DBContext;
using BudganInfra.DBContext.Tables;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.GetSpan;

internal class GetSpanRecurringTransactionsRepoOp
    : BaseRepositoryOperationWithResultValue<DaoRecurringTransactionsSpan>, IGetSpanRecurringTransactionsRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public GetSpanRecurringTransactionsRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var dates = await this._dataContext.AccountTransactions
            .Where(x => x.AccountId == this._accountId
                        && x.RecordType == AccountTransactionRecordType.Normal
                        && this._dataContext.AccountRecurringTransactions
                            .Any(r => r.AccountId == this._accountId && r.RecurringId == x.RecurringId))
            .Select(x => x.DateInscription)
            .ToListAsync();

        if (dates.Count == 0)
        {
            this.SetFailed();
            return;
        }

        this.SetSucceeded(new DaoRecurringTransactionsSpan
        {
            Start = dates.Min(),
            End = dates.Max(),
        });
    }
}
