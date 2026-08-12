using System.Globalization;
using BudganInfra.Repositories.AccountTransaction;

namespace BudganServices.UseCases.AccountTransaction.GetList;

internal class ListAccountTransactionUseCase : BaseUseCaseWithResultValue<List<BOListAccountTransaction>>, IListAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;

    public ListAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository)
    {
        this._accountTransactionRepository = accountTransactionRepository;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountTransactionRepository.ListAccountTransactionRepoOperation();

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
                RecordType = AccountTransactionRecordTypeConverter.FromDao(x.RecordType),
            })
            .ToList();

        this.SetSucceeded(result);
    }
}
