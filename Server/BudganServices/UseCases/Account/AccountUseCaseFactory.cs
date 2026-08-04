using BudganInfra.Repositories.Account;
using BudganServices.UseCases.Account.AddOrUpdate;
using BudganServices.UseCases.Account.Delete;
using BudganServices.UseCases.Account.Get;
using BudganServices.UseCases.Account.GetList;

namespace BudganServices.UseCases.Account;

internal class AccountUseCaseFactory : IAccountUseCaseFactory
{
    private readonly IAccountRepository _accountRepository;

    public AccountUseCaseFactory(IAccountRepository accountRepository)
    {
        this._accountRepository = accountRepository;
    }

    public IAddOrUpdateAccountUseCase AddOrUpdateUseCase(BOAddOrUpdateAccount model)
    {
        return new AddOrUpdateAccountUseCase(this._accountRepository, model);
    }

    public IListAccountUseCase ListAccountUseCase()
    {
        return new ListAccountUseCase(this._accountRepository);
    }

    public IGetAccountUseCase GetAccountUseCase(Guid id)
    {
        return new GetAccountUseCase(this._accountRepository, id);
    }

    public IDeleteAccountUseCase DeleteAccountUseCase(Guid id)
    {
        return new DeleteAccountUseCase(this._accountRepository, id);
    }
}
