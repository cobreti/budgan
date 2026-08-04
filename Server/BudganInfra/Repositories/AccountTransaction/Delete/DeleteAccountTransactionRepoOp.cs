using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.Delete;

public class DeleteAccountTransactionRepoOp : BaseRepositoryOperationWithResultValue<Guid>, IDeleteAccountTransactionRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _id;

    public DeleteAccountTransactionRepoOp(DataContext dataContext, Guid id)
    {
        this._dataContext = dataContext;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.AccountTransactions
            .FirstOrDefaultAsync(x => x.Id == this._id);

        if (entity == null)
        {
            this.SetFailed();
        }
        else
        {
            this._dataContext.AccountTransactions.Remove(entity);
            await this._dataContext.SaveChangesAsync();

            this.SetSucceeded(this._id);
        }
    }
}
