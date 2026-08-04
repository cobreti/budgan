using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.GetList;

internal class ListAccountTransactionRepoOp
    : BaseRepositoryOperationWithResultValue<List<DaoListAccountTransaction>>, IListAccountTransactionRepoOp
{
    private readonly DataContext _dataContext;

    public ListAccountTransactionRepoOp(DataContext dataContext)
    {
        this._dataContext = dataContext;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.AccountTransactions
            .Select(x => new DaoListAccountTransaction
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
