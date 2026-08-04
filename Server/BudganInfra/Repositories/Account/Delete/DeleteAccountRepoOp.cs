using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.Account.Delete;

public class DeleteAccountRepoOp : BaseRepositoryOperationWithResultValue<Guid>, IDeleteAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _id;

    public DeleteAccountRepoOp(DataContext dataContext, Guid id)
    {
        this._dataContext = dataContext;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.Accounts
            .FirstOrDefaultAsync(a => a.Id == this._id);

        if (entity == null)
        {
            this.SetFailed();
        }
        else
        {
            this._dataContext.Accounts.Remove(entity);
            await this._dataContext.SaveChangesAsync();

            this.SetSucceeded(this._id);
        }
    }
}
