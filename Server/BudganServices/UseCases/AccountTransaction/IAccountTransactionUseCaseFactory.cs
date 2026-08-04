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

public interface IAccountTransactionUseCaseFactory
{
    ICreateAccountTransactionUseCase CreateUseCase(BOCreateAccountTransaction model);
    IGetAccountTransactionUseCase GetUseCase(Guid id);
    IDeleteAccountTransactionUseCase DeleteUseCase(Guid id);
    IListAccountTransactionByAccountUseCase ListByAccountUseCase(Guid accountId);
    IGetCountAccountTransactionByAccountUseCase GetCountByAccountUseCase(Guid accountId);

    IGetPageAccountTransactionByAccountUseCase GetPageByAccountUseCase(
        Guid accountId,
        int page,
        int pageSize,
        AccountTransactionSortField sortField,
        SortDirection sortDirection);

    IGetSnapshotAccountTransactionUseCase GetSnapshotUseCase(Guid accountId);
    ISetSnapshotAccountTransactionUseCase SetSnapshotUseCase(BOSetAccountTransactionSnapshot model);
    IDeleteSnapshotAccountTransactionUseCase DeleteSnapshotUseCase(Guid accountId);
    IRecalculateBalancesAccountTransactionUseCase RecalculateBalancesUseCase(Guid accountId);
}
