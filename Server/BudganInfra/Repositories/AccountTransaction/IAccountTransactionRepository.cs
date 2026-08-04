using BudganInfra.Repositories.AccountTransaction.Delete;
using BudganInfra.Repositories.AccountTransaction.Get;
using BudganInfra.Repositories.AccountTransaction.GetCountByAccount;
using BudganInfra.Repositories.AccountTransaction.GetList;
using BudganInfra.Repositories.AccountTransaction.GetListByAccount;
using BudganInfra.Repositories.AccountTransaction.GetPageByAccount;
using BudganInfra.Repositories.AccountTransaction.GetSnapshot;
using BudganInfra.Repositories.AccountTransaction.Save;
using BudganInfra.Repositories.AccountTransaction.UpdateBalances;

namespace BudganInfra.Repositories.AccountTransaction;

public interface IAccountTransactionRepository
{
    ISaveAccountTransactionRepoOp SaveAccountTransactionRepoOperation(DaoSaveAccountTransaction daoSaveAccountTransaction);
    IGetAccountTransactionRepoOp GetAccountTransactionRepoOperation(Guid id);
    IDeleteAccountTransactionRepoOp DeleteAccountTransactionRepoOperation(Guid id);
    IListAccountTransactionRepoOp ListAccountTransactionRepoOperation();
    IListAccountTransactionByAccountRepoOp ListAccountTransactionByAccountRepoOperation(Guid accountId);
    IGetCountAccountTransactionByAccountRepoOp GetCountAccountTransactionByAccountRepoOperation(Guid accountId);

    IGetPageAccountTransactionByAccountRepoOp GetPageAccountTransactionByAccountRepoOperation(
        Guid accountId,
        int page,
        int pageSize,
        AccountTransactionSortField sortField,
        SortDirection sortDirection);

    IGetSnapshotAccountTransactionByAccountRepoOp GetSnapshotAccountTransactionByAccountRepoOperation(Guid accountId);

    IUpdateBalancesAccountTransactionRepoOp UpdateBalancesAccountTransactionRepoOperation(
        List<DaoAccountTransactionBalanceUpdate> updates);
}
