using System.Globalization;
using BudganInfra.Repositories.AccountTransaction;

namespace BudganServices.UseCases.AccountTransaction.GetListByAccount;

internal class ListAccountTransactionByAccountUseCase : BaseUseCaseWithResultValue<List<BOListAccountTransaction>>, IListAccountTransactionByAccountUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _accountId;

    public ListAccountTransactionByAccountUseCase(IAccountTransactionRepository accountTransactionRepository, Guid accountId)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountTransactionRepository.ListAccountTransactionByAccountRepoOperation(this._accountId);

        await repoOp.ExecuteAsync();

        var result = repoOp.ResultValue
            .Select(x => new BOListAccountTransaction
            {
                Id = x.Id.ToString(),
                AccountId = x.AccountId,
                FileId = x.FileId,
                CardNumber = x.CardNumber,
                DateInscriptionAsString = x.DateInscription.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                Amount = x.Amount,
                Balance = x.Balance,
                BalanceDateOffset = x.BalanceDateOffset,
                Description = x.Description,
                UniqueKey = x.UniqueKey,
                RecurringId = x.RecurringId,
                RecordType = x.RecordType,
            })
            .ToList();

        this.SetSucceeded(result);
    }
}
