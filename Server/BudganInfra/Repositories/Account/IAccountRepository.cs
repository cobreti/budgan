using BudganInfra.Repositories.Account.Delete;
using BudganInfra.Repositories.Account.Get;
using BudganInfra.Repositories.Account.GetList;
using BudganInfra.Repositories.Account.Save;

namespace BudganInfra.Repositories.Account;

public interface IAccountRepository
{
    ISaveAccountRepoOp SaveAccountRepoOperation(DaoSaveAccount daoSaveAccount);
    IListAccountRepoOp ListAccountRepoOperation();
    IGetAccountRepoOp GetAccountRepoOperation(Guid id);
    IDeleteAccountRepoOp DeleteAccountRepoOperation(Guid id);
}
