using System.Globalization;
using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganInfra.Repositories.AccountRecurringTransaction;
using BudganInfra.Repositories.AccountRecurringTransaction.Insert;

namespace BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;

internal class ReplaceAccountRecurringTransactionsForAccountUseCase : BaseUseCase, IReplaceAccountRecurringTransactionsForAccountUseCase
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;
    private readonly BOReplaceAccountRecurringTransactionsForAccount _model;

    public ReplaceAccountRecurringTransactionsForAccountUseCase(
        IAccountRecurringTransactionRepository accountRecurringTransactionRepository,
        BOReplaceAccountRecurringTransactionsForAccount model)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
        this._model = model;
    }

    public async Task ExecuteAsync()
    {
        var daoItems = this._model.Items
            .Select(item => new DaoInsertAccountRecurringTransaction
            {
                AccountId = this._model.AccountId,
                RecurringId = item.RecurringId,
                PeriodInDays = item.PeriodInDays,
                TransactionCount = item.TransactionCount,
                Description = item.Description,
                AverageAmount = item.AverageAmount,
                FirstOccurrenceDate = DateOnly.ParseExact(item.FirstOccurrenceDateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                LastOccurrenceDate = DateOnly.ParseExact(item.LastOccurrenceDateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture),
            })
            .ToList();

        var repoOp = this._accountRecurringTransactionRepository.ReplaceAccountRecurringTransactionsForAccountRepoOperation(
            this._model.AccountId,
            daoItems);

        await repoOp.ExecuteAsync();

        if (!repoOp.Succeeded)
        {
            // ReplaceAccountRecurringTransactionsForAccountRepoOp's only non-throwing failure
            // path is a duplicate RecurringId unique-constraint violation; other failures throw directly.
            throw new BudganException(BudganErrorValue.DuplicateAccountRecurringTransaction);
        }

        this.SetSucceeded();
    }
}
