using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.Account.GetList;

internal class ListAccountRepoOp : BaseRepositoryOperationWithResultValue<List<DaoListAccount>>, IListAccountRepoOp
{
    private readonly DataContext _dataContext;

    public ListAccountRepoOp(DataContext dataContext)
    {
        this._dataContext = dataContext;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.Accounts
            .Select(x => new DaoListAccount
            {
                Id = x.Id,
                Name = x.Name,
                ColumnsMappingId = x.ColumnsMappingId,
                AccountType = x.AccountType
            })
            .ToListAsync();

        this.SetSucceeded(result);
    }
}
