using BudganInfra.Repositories.AccountRecurringTransaction;
using BudganServices.UseCases.AccountRecurringTransaction.DeleteByAccount;
using BudganServices.UseCases.AccountRecurringTransaction.GetList;
using BudganServices.UseCases.AccountRecurringTransaction.GetListByAccount;
using BudganServices.UseCases.AccountRecurringTransaction.GetSpan;
using BudganServices.UseCases.AccountRecurringTransaction.GetTransactionsByAccount;
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

    public IGetRecurringTransactionsByAccountUseCase GetTransactionsByAccountUseCase(
        Guid accountId,
        DateOnly startDate,
        DateOnly endDate)
    {
        return new GetRecurringTransactionsByAccountUseCase(this._accountRecurringTransactionRepository, accountId, startDate, endDate);
    }

    public IGetSpanRecurringTransactionsUseCase GetSpanUseCase(Guid accountId)
    {
        return new GetSpanRecurringTransactionsUseCase(this._accountRecurringTransactionRepository, accountId);
    }

    public IListAccountRecurringTransactionByAccountUseCase ListByAccountUseCase(Guid accountId)
    {
        return new ListAccountRecurringTransactionByAccountUseCase(this._accountRecurringTransactionRepository, accountId);
    }

    public IListAccountRecurringTransactionUseCase ListUseCase()
    {
        return new ListAccountRecurringTransactionUseCase(this._accountRecurringTransactionRepository);
    }

    public IDeleteAccountRecurringTransactionByAccountUseCase DeleteByAccountUseCase(Guid accountId)
    {
        return new DeleteAccountRecurringTransactionByAccountUseCase(this._accountRecurringTransactionRepository, accountId);
    }
}
