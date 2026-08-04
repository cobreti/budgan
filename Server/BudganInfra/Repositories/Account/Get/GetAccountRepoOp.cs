using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.Account.Get;

public class GetAccountRepoOp : BaseRepositoryOperationWithResultValue<DaoGetAccount>, IGetAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _id;

    public GetAccountRepoOp(DataContext dataContext, Guid id)
    {
        this._dataContext = dataContext;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.Accounts
            .FirstOrDefaultAsync(x => x.Id == this._id);

        if (entity != null)
        {
            this.SetSucceeded(new DaoGetAccount
            {
                Id = entity.Id.ToString(),
                Timestamp = entity.Timestamp,
                Name = entity.Name,
                ColumnsMappingId = entity.ColumnsMappingId,
                AccountType = entity.AccountType,
            });
        }
        else
        {
            this.SetFailed();
        }
    }
}
