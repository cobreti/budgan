using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.TransactionsFile.GetListByAccount;

internal class GetListByAccountTransactionsFileRepoOp : BaseRepositoryOperationWithResultValue<List<DaoTransactionsFile>>, IGetListByAccountTransactionsFileRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public GetListByAccountTransactionsFileRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.TransactionsFiles
            .Where(x => x.AccountId == this._accountId)
            .Select(x => new DaoTransactionsFile
            {
                Id = x.Id,
                AccountId = x.AccountId,
                Filename = x.Filename,
                InsertionDate = x.InsertionDate,
            })
            .ToListAsync();

        this.SetSucceeded(result);
    }
}
