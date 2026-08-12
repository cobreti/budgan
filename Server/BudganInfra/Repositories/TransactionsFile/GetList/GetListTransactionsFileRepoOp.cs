using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.TransactionsFile.GetList;

internal class GetListTransactionsFileRepoOp : BaseRepositoryOperationWithResultValue<List<DaoTransactionsFile>>, IGetListTransactionsFileRepoOp
{
    private readonly DataContext _dataContext;

    public GetListTransactionsFileRepoOp(DataContext dataContext)
    {
        this._dataContext = dataContext;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.TransactionsFiles
            .Select(x => new DaoTransactionsFile
            {
                Id = x.Id,
                AccountId = x.AccountId,
                Content = x.Content,
                Filename = x.Filename,
                InsertionDate = x.InsertionDate,
            })
            .ToListAsync();

        this.SetSucceeded(result);
    }
}
