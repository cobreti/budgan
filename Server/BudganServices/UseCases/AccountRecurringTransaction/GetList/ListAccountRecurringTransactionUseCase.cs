using System.Globalization;
using BudganInfra.Repositories.AccountRecurringTransaction;

namespace BudganServices.UseCases.AccountRecurringTransaction.GetList;

internal class ListAccountRecurringTransactionUseCase
    : BaseUseCaseWithResultValue<List<BOListAccountRecurringTransaction>>, IListAccountRecurringTransactionUseCase
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;

    public ListAccountRecurringTransactionUseCase(IAccountRecurringTransactionRepository accountRecurringTransactionRepository)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRecurringTransactionRepository.ListAccountRecurringTransactionRepoOperation();

        await repoOp.ExecuteAsync();

        var result = repoOp.ResultValue
            .Select(x => new BOListAccountRecurringTransaction
            {
                AccountId = x.AccountId,
                RecurringId = x.RecurringId,
                PeriodInDays = x.PeriodInDays,
                TransactionCount = x.TransactionCount,
                Description = x.Description,
                AverageAmount = x.AverageAmount,
                FirstOccurrenceDateAsString = x.FirstOccurrenceDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                LastOccurrenceDateAsString = x.LastOccurrenceDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            })
            .ToList();

        this.SetSucceeded(result);
    }
}
