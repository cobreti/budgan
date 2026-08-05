using System.Globalization;
using BudganInfra.Repositories.AccountRecurringTransaction;
using BudganServices.UseCases.AccountTransaction;

namespace BudganServices.UseCases.AccountRecurringTransaction.GetTransactionsByAccount;

internal class GetRecurringTransactionsByAccountUseCase
    : BaseUseCaseWithResultValue<List<BOAccountRecurringTransaction>>, IGetRecurringTransactionsByAccountUseCase
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;
    private readonly Guid _accountId;
    private readonly DateOnly _startDate;
    private readonly DateOnly _endDate;

    public GetRecurringTransactionsByAccountUseCase(
        IAccountRecurringTransactionRepository accountRecurringTransactionRepository,
        Guid accountId,
        DateOnly startDate,
        DateOnly endDate)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
        this._accountId = accountId;
        this._startDate = startDate;
        this._endDate = endDate;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRecurringTransactionRepository.GetTransactionsByAccountRecurringRepoOperation(
            this._accountId, this._startDate, this._endDate);

        await repoOp.ExecuteAsync();

        var result = repoOp.ResultValue
            .Select(x => new BOAccountRecurringTransaction
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
