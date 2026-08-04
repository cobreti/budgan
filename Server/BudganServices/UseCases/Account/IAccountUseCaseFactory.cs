using BudganServices.UseCases.Account.AddOrUpdate;
using BudganServices.UseCases.Account.Delete;
using BudganServices.UseCases.Account.Get;
using BudganServices.UseCases.Account.GetList;

namespace BudganServices.UseCases.Account;

public interface IAccountUseCaseFactory
{
    IAddOrUpdateAccountUseCase AddOrUpdateUseCase(BOAddOrUpdateAccount model);
    IListAccountUseCase ListAccountUseCase();
    IGetAccountUseCase GetAccountUseCase(Guid id);
    IDeleteAccountUseCase DeleteAccountUseCase(Guid id);
}
