using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.GetListByAccount;

internal class ListAccountRecurringTransactionByAccountRepoOp
    : BaseRepositoryOperationWithResultValue<List<DaoListAccountRecurringTransactionByAccount>>, IListAccountRecurringTransactionByAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public ListAccountRecurringTransactionByAccountRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.AccountRecurringTransactions
            .Where(x => x.AccountId == this._accountId)
            .Select(x => new DaoListAccountRecurringTransactionByAccount
            {
                Id = x.Id,
                AccountId = x.AccountId,
                RecurringId = x.RecurringId,
                PeriodInDays = x.PeriodInDays,
                TransactionCount = x.TransactionCount,
                Description = x.Description,
                AverageAmount = x.AverageAmount,
                FirstOccurrenceDate = x.FirstOccurrenceDate,
                LastOccurrenceDate = x.LastOccurrenceDate,
            })
            .ToListAsync();

        this.SetSucceeded(result);
    }
}
