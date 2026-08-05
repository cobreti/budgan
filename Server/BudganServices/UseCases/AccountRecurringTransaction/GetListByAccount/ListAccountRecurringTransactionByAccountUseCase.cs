using System.Globalization;
using BudganInfra.Repositories.AccountRecurringTransaction;
using BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;

namespace BudganServices.UseCases.AccountRecurringTransaction.GetListByAccount;

internal class ListAccountRecurringTransactionByAccountUseCase
    : BaseUseCaseWithResultValue<List<BOAccountRecurringTransactionItem>>, IListAccountRecurringTransactionByAccountUseCase
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;
    private readonly Guid _accountId;

    public ListAccountRecurringTransactionByAccountUseCase(IAccountRecurringTransactionRepository accountRecurringTransactionRepository, Guid accountId)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRecurringTransactionRepository.ListAccountRecurringTransactionByAccountRepoOperation(this._accountId);

        await repoOp.ExecuteAsync();

        var result = repoOp.ResultValue
            .Select(x => new BOAccountRecurringTransactionItem
            {
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
