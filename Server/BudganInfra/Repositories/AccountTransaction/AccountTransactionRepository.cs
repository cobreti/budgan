using BudganInfra.DBContext;
using BudganInfra.Repositories.AccountTransaction.Delete;
using BudganInfra.Repositories.AccountTransaction.Get;
using BudganInfra.Repositories.AccountTransaction.GetCountByAccount;
using BudganInfra.Repositories.AccountTransaction.GetList;
using BudganInfra.Repositories.AccountTransaction.GetListByAccount;
using BudganInfra.Repositories.AccountTransaction.GetPageByAccount;
using BudganInfra.Repositories.AccountTransaction.GetSnapshot;
using BudganInfra.Repositories.AccountTransaction.Save;
using BudganInfra.Repositories.AccountTransaction.UpdateBalances;
using Microsoft.Extensions.Logging;

namespace BudganInfra.Repositories.AccountTransaction;

internal class AccountTransactionRepository : IAccountTransactionRepository
{
    private readonly DataContext _dataContext;
    private readonly ILogger<AccountTransactionRepository> _logger;

    public AccountTransactionRepository(ILogger<AccountTransactionRepository> logger, DataContext dataContext)
    {
        this._logger = logger;
        this._dataContext = dataContext;
    }

    public ISaveAccountTransactionRepoOp SaveAccountTransactionRepoOperation(DaoSaveAccountTransaction daoSaveAccountTransaction)
    {
        return new SaveAccountTransactionRepoOp(this._dataContext, daoSaveAccountTransaction);
    }

    public IGetAccountTransactionRepoOp GetAccountTransactionRepoOperation(Guid id)
    {
        return new GetAccountTransactionRepoOp(this._dataContext, id);
    }

    public IDeleteAccountTransactionRepoOp DeleteAccountTransactionRepoOperation(Guid id)
    {
        return new DeleteAccountTransactionRepoOp(this._dataContext, id);
    }

    public IListAccountTransactionRepoOp ListAccountTransactionRepoOperation()
    {
        return new ListAccountTransactionRepoOp(this._dataContext);
    }

    public IListAccountTransactionByAccountRepoOp ListAccountTransactionByAccountRepoOperation(Guid accountId)
    {
        return new ListAccountTransactionByAccountRepoOp(this._dataContext, accountId);
    }

    public IGetCountAccountTransactionByAccountRepoOp GetCountAccountTransactionByAccountRepoOperation(Guid accountId)
    {
        return new GetCountAccountTransactionByAccountRepoOp(this._dataContext, accountId);
    }

    public IGetPageAccountTransactionByAccountRepoOp GetPageAccountTransactionByAccountRepoOperation(
        Guid accountId,
        int page,
        int pageSize,
        AccountTransactionSortField sortField,
        SortDirection sortDirection)
    {
        return new GetPageAccountTransactionByAccountRepoOp(this._dataContext, accountId, page, pageSize, sortField, sortDirection);
    }

    public IGetSnapshotAccountTransactionByAccountRepoOp GetSnapshotAccountTransactionByAccountRepoOperation(Guid accountId)
    {
        return new GetSnapshotAccountTransactionByAccountRepoOp(this._dataContext, accountId);
    }

    public IUpdateBalancesAccountTransactionRepoOp UpdateBalancesAccountTransactionRepoOperation(
        List<DaoAccountTransactionBalanceUpdate> updates)
    {
        return new UpdateBalancesAccountTransactionRepoOp(this._dataContext, updates);
    }
}
