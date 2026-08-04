using System.Globalization;
using BudganInfra.Repositories.AccountTransaction;

namespace BudganServices.UseCases.AccountTransaction.Get;

internal class GetAccountTransactionUseCase : BaseUseCaseWithResultValue<BOGetAccountTransaction>, IGetAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _id;

    public GetAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, Guid id)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountTransactionRepository.GetAccountTransactionRepoOperation(this._id);

        await repoOp.ExecuteAsync();

        if (repoOp.Succeeded)
        {
            var r = repoOp.ResultValue;

            this.SetSucceeded(new BOGetAccountTransaction
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
