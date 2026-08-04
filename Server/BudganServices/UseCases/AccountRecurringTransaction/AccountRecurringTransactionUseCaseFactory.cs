using BudganInfra.Repositories.AccountRecurringTransaction;
using BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;

namespace BudganServices.UseCases.AccountRecurringTransaction;

internal class AccountRecurringTransactionUseCaseFactory : IAccountRecurringTransactionUseCaseFactory
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;

    public AccountRecurringTransactionUseCaseFactory(IAccountRecurringTransactionRepository accountRecurringTransactionRepository)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
    }

    public IReplaceAccountRecurringTransactionsForAccountUseCase ReplaceForAccountUseCase(BOReplaceAccountRecurringTransactionsForAccount model)
    {
        return new ReplaceAccountRecurringTransactionsForAccountUseCase(this._accountRecurringTransactionRepository, model);
    }
}
