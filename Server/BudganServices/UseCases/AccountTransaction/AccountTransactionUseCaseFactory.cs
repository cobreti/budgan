using BudganInfra.Repositories.AccountTransaction;
using BudganInfra.Repositories.AccountTransaction.GetPageByAccount;
using BudganServices.UseCases.AccountTransaction.Create;
using BudganServices.UseCases.AccountTransaction.Delete;
using BudganServices.UseCases.AccountTransaction.DeleteSnapshot;
using BudganServices.UseCases.AccountTransaction.Get;
using BudganServices.UseCases.AccountTransaction.GetCountByAccount;
using BudganServices.UseCases.AccountTransaction.GetListByAccount;
using BudganServices.UseCases.AccountTransaction.GetPageByAccount;
using BudganServices.UseCases.AccountTransaction.GetSnapshot;
using BudganServices.UseCases.AccountTransaction.RecalculateBalances;
using BudganServices.UseCases.AccountTransaction.SetSnapshot;

namespace BudganServices.UseCases.AccountTransaction;

internal class AccountTransactionUseCaseFactory : IAccountTransactionUseCaseFactory
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;

    public AccountTransactionUseCaseFactory(IAccountTransactionRepository accountTransactionRepository)
    {
        this._accountTransactionRepository = accountTransactionRepository;
    }

    public ICreateAccountTransactionUseCase CreateUseCase(BOCreateAccountTransaction model)
    {
        return new CreateAccountTransactionUseCase(this._accountTransactionRepository, model);
    }

    public IGetAccountTransactionUseCase GetUseCase(Guid id)
    {
        return new GetAccountTransactionUseCase(this._accountTransactionRepository, id);
    }

    public IDeleteAccountTransactionUseCase DeleteUseCase(Guid id)
    {
        return new DeleteAccountTransactionUseCase(this._accountTransactionRepository, id);
    }

    public IListAccountTransactionByAccountUseCase ListByAccountUseCase(Guid accountId)
    {
        return new ListAccountTransactionByAccountUseCase(this._accountTransactionRepository, accountId);
    }

    public IGetCountAccountTransactionByAccountUseCase GetCountByAccountUseCase(Guid accountId)
    {
        return new GetCountAccountTransactionByAccountUseCase(this._accountTransactionRepository, accountId);
    }

    public IGetPageAccountTransactionByAccountUseCase GetPageByAccountUseCase(
        Guid accountId,
        int page,
        int pageSize,
        AccountTransactionSortField sortField,
        SortDirection sortDirection)
    {
        return new GetPageAccountTransactionByAccountUseCase(this._accountTransactionRepository, accountId, page, pageSize, sortField, sortDirection);
    }

    public IGetSnapshotAccountTransactionUseCase GetSnapshotUseCase(Guid accountId)
    {
        return new GetSnapshotAccountTransactionUseCase(this._accountTransactionRepository, accountId);
    }

    public ISetSnapshotAccountTransactionUseCase SetSnapshotUseCase(BOSetAccountTransactionSnapshot model)
    {
        return new SetSnapshotAccountTransactionUseCase(this._accountTransactionRepository, model);
    }

    public IDeleteSnapshotAccountTransactionUseCase DeleteSnapshotUseCase(Guid accountId)
    {
        return new DeleteSnapshotAccountTransactionUseCase(this._accountTransactionRepository, accountId);
    }

    public IRecalculateBalancesAccountTransactionUseCase RecalculateBalancesUseCase(Guid accountId)
    {
        return new RecalculateBalancesAccountTransactionUseCase(this._accountTransactionRepository, accountId);
    }
}
