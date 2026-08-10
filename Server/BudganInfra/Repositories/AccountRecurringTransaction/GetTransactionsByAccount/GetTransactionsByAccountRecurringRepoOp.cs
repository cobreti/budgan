using BudganInfra.DBContext;
using BudganInfra.DBContext.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.GetTransactionsByAccount;

internal class GetTransactionsByAccountRecurringRepoOp
    : BaseRepositoryOperationWithResultValue<List<DaoAccountTransactionByRecurring>>, IGetTransactionsByAccountRecurringRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;
    private readonly DateOnly _startDate;
    private readonly DateOnly _endDate;

    public GetTransactionsByAccountRecurringRepoOp(DataContext dataContext, Guid accountId, DateOnly startDate, DateOnly endDate)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
        this._startDate = startDate;
        this._endDate = endDate;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.AccountTransactions
            .Where(x => x.AccountId == this._accountId
                        && x.RecordType == AccountTransactionRecordType.Normal
                        && x.DateInscription >= this._startDate
                        && x.DateInscription <= this._endDate
                        && this._dataContext.AccountRecurringTransactions
                            .Any(r => r.AccountId == this._accountId && r.RecurringId == x.RecurringId))
            .Select(x => new DaoAccountTransactionByRecurring
            {
                Id = x.Id,
                AccountId = x.AccountId,
                UniqueKey = x.UniqueKey,
                RecurringId = x.RecurringId,
                FileId = x.FileId,
                CardNumber = x.CardNumber,
                DateInscription = x.DateInscription,
                Amount = x.Amount,
                Balance = x.Balance,
                BalanceDateOffset = x.BalanceDateOffset,
                Description = x.Description,
                RecordType = x.RecordType,
            })
            .OrderBy(x => x.DateInscription)
            .ToListAsync();

        this.SetSucceeded(result);
    }
}
