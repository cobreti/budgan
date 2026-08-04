using System.Globalization;
using BudganInfra.Repositories.AccountTransaction;

namespace BudganServices.UseCases.AccountTransaction.GetSnapshot;

internal class GetSnapshotAccountTransactionUseCase : BaseUseCaseWithResultValue<BOGetAccountTransactionSnapshot>, IGetSnapshotAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _accountId;

    public GetSnapshotAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, Guid accountId)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountTransactionRepository.GetSnapshotAccountTransactionByAccountRepoOperation(this._accountId);

        await repoOp.ExecuteAsync();

        if (repoOp.Succeeded)
        {
            var r = repoOp.ResultValue;

            this.SetSucceeded(new BOGetAccountTransactionSnapshot
            {
                Id = r.Id!,
                Timestamp = r.Timestamp!.Value,
                AccountId = r.AccountId,
                FileId = r.FileId,
                CardNumber = r.CardNumber,
                DateInscriptionAsString = r.DateInscription.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Amount = r.Amount,
                Balance = r.Balance,
                BalanceDateOffset = r.BalanceDateOffset,
                Description = r.Description,
                UniqueKey = r.UniqueKey,
                RecurringId = r.RecurringId,
                RecordType = r.RecordType,
            });
        }
        else
        {
            this.SetFailed();
        }
    }
}
