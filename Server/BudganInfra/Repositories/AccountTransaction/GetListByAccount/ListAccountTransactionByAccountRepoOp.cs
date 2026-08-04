using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.GetListByAccount;

internal class ListAccountTransactionByAccountRepoOp
    : BaseRepositoryOperationWithResultValue<List<DaoListAccountTransactionByAccount>>, IListAccountTransactionByAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public ListAccountTransactionByAccountRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.AccountTransactions
            .Where(x => x.AccountId == this._accountId)
            .Select(x => new DaoListAccountTransactionByAccount
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
            .ToListAsync();

        this.SetSucceeded(result);
    }
}
