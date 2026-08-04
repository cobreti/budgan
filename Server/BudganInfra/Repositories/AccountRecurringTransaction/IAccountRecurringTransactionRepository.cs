using BudganInfra.Repositories.AccountRecurringTransaction.DeleteByAccount;
using BudganInfra.Repositories.AccountRecurringTransaction.GetList;
using BudganInfra.Repositories.AccountRecurringTransaction.GetListByAccount;
using BudganInfra.Repositories.AccountRecurringTransaction.Insert;
using BudganInfra.Repositories.AccountRecurringTransaction.ReplaceForAccount;

namespace BudganInfra.Repositories.AccountRecurringTransaction;

public interface IAccountRecurringTransactionRepository
{
    IInsertAccountRecurringTransactionRepoOp InsertAccountRecurringTransactionRepoOperation(
        List<DaoInsertAccountRecurringTransaction> items);

    IListAccountRecurringTransactionRepoOp ListAccountRecurringTransactionRepoOperation();

    IListAccountRecurringTransactionByAccountRepoOp ListAccountRecurringTransactionByAccountRepoOperation(Guid accountId);

    IDeleteAccountRecurringTransactionByAccountRepoOp DeleteAccountRecurringTransactionByAccountRepoOperation(Guid accountId);

    IReplaceAccountRecurringTransactionsForAccountRepoOp ReplaceAccountRecurringTransactionsForAccountRepoOperation(
        Guid accountId,
        List<DaoInsertAccountRecurringTransaction> items);
}
