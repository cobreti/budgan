using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.GetList;

internal class ListAccountRecurringTransactionRepoOp
    : BaseRepositoryOperationWithResultValue<List<DaoListAccountRecurringTransaction>>, IListAccountRecurringTransactionRepoOp
{
    private readonly DataContext _dataContext;

    public ListAccountRecurringTransactionRepoOp(DataContext dataContext)
    {
        this._dataContext = dataContext;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.AccountRecurringTransactions
            .Select(x => new DaoListAccountRecurringTransaction
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
