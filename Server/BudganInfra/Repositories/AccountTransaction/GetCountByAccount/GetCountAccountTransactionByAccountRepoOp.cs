using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.GetCountByAccount;

public class GetCountAccountTransactionByAccountRepoOp
    : BaseRepositoryOperationWithResultValue<DaoGetCountAccountTransaction>, IGetCountAccountTransactionByAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public GetCountAccountTransactionByAccountRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var count = await this._dataContext.AccountTransactions
            .CountAsync(x => x.AccountId == this._accountId);

        this.SetSucceeded(new DaoGetCountAccountTransaction { Count = count });
    }
}
