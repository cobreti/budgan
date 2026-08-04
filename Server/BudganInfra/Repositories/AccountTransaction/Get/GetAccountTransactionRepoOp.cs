using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.Get;

public class GetAccountTransactionRepoOp : BaseRepositoryOperationWithResultValue<DaoGetAccountTransaction>, IGetAccountTransactionRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _id;

    public GetAccountTransactionRepoOp(DataContext dataContext, Guid id)
    {
        this._dataContext = dataContext;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.AccountTransactions
            .FirstOrDefaultAsync(x => x.Id == this._id);

        if (entity != null)
        {
            this.SetSucceeded(new DaoGetAccountTransaction
            {
                Id = entity.Id.ToString(),
                Timestamp = entity.Timestamp,
                AccountId = entity.AccountId,
                UniqueKey = entity.UniqueKey,
                RecurringId = entity.RecurringId,
                FileId = entity.FileId,
                CardNumber = entity.CardNumber,
                DateInscription = entity.DateInscription,
                Amount = entity.Amount,
                Balance = entity.Balance,
                BalanceDateOffset = entity.BalanceDateOffset,
                Description = entity.Description,
                RecordType = entity.RecordType,
            });
        }
        else
        {
            this.SetFailed();
        }
    }
}
