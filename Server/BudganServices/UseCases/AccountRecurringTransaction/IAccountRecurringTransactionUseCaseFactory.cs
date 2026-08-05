using BudganServices.UseCases.AccountRecurringTransaction.DeleteByAccount;
using BudganServices.UseCases.AccountRecurringTransaction.GetListByAccount;
using BudganServices.UseCases.AccountRecurringTransaction.GetSpan;
using BudganServices.UseCases.AccountRecurringTransaction.GetTransactionsByAccount;
using BudganServices.UseCases.AccountRecurringTransaction.ReplaceForAccount;

namespace BudganServices.UseCases.AccountRecurringTransaction;

public interface IAccountRecurringTransactionUseCaseFactory
{
    IReplaceAccountRecurringTransactionsForAccountUseCase ReplaceForAccountUseCase(BOReplaceAccountRecurringTransactionsForAccount model);

    IGetRecurringTransactionsByAccountUseCase GetTransactionsByAccountUseCase(
        Guid accountId,
        DateOnly startDate,
        DateOnly endDate);

    IGetSpanRecurringTransactionsUseCase GetSpanUseCase(Guid accountId);

    IListAccountRecurringTransactionByAccountUseCase ListByAccountUseCase(Guid accountId);

    IDeleteAccountRecurringTransactionByAccountUseCase DeleteByAccountUseCase(Guid accountId);
}
