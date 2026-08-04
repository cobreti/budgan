using BudganInfra.DBContext;
using BudganInfra.DBContext.Tables;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.GetSnapshot;

public class GetSnapshotAccountTransactionByAccountRepoOp
    : BaseRepositoryOperationWithResultValue<DaoGetSnapshotAccountTransaction>, IGetSnapshotAccountTransactionByAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;

    public GetSnapshotAccountTransactionByAccountRepoOp(DataContext dataContext, Guid accountId)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.AccountTransactions
            .FirstOrDefaultAsync(x => x.AccountId == this._accountId
                                       && x.RecordType == AccountTransactionRecordType.Snapshot);

        if (entity != null)
        {
            this.SetSucceeded(new DaoGetSnapshotAccountTransaction
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
