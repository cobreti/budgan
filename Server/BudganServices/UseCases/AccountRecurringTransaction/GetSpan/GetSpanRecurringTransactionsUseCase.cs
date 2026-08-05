using System.Globalization;
using BudganInfra.Repositories.AccountRecurringTransaction;

namespace BudganServices.UseCases.AccountRecurringTransaction.GetSpan;

internal class GetSpanRecurringTransactionsUseCase
    : BaseUseCaseWithResultValue<BOGetRecurringTransactionsSpan>, IGetSpanRecurringTransactionsUseCase
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;
    private readonly Guid _accountId;

    public GetSpanRecurringTransactionsUseCase(IAccountRecurringTransactionRepository accountRecurringTransactionRepository, Guid accountId)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRecurringTransactionRepository.GetSpanRecurringTransactionsRepoOperation(this._accountId);

        await repoOp.ExecuteAsync();

        if (repoOp.Succeeded)
        {
            var r = repoOp.ResultValue;

            this.SetSucceeded(new BOGetRecurringTransactionsSpan
            {
                StartAsString = r.Start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EndAsString = r.End.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            });
        }
        else
        {
            this.SetFailed();
        }
    }
}
