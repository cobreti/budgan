using BudganInfra.Repositories.Account;

namespace BudganServices.UseCases.Account.Delete;

internal class DeleteAccountUseCase : BaseUseCaseWithResultValue<Guid>, IDeleteAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly Guid _id;

    public DeleteAccountUseCase(IAccountRepository accountRepository, Guid id)
    {
        this._accountRepository = accountRepository;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRepository.DeleteAccountRepoOperation(this._id);

        await repoOp.ExecuteAsync();

        if (repoOp.Succeeded)
        {
            this.SetSucceeded(this._id);
        }
        else
        {
            this.SetFailed();
        }
    }
}
