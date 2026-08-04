using BudganInfra.Repositories.Account;

namespace BudganServices.UseCases.Account.Get;

internal class GetAccountUseCase : BaseUseCaseWithResultValue<BOGetAccount>, IGetAccountUseCase
{
    private readonly IAccountRepository _accountRepository;
    private readonly Guid _id;

    public GetAccountUseCase(IAccountRepository accountRepository, Guid id)
    {
        this._accountRepository = accountRepository;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRepository.GetAccountRepoOperation(this._id);

        await repoOp.ExecuteAsync();

        if (repoOp.Succeeded)
        {
            var r = repoOp.ResultValue;

            this.SetSucceeded(new BOGetAccount
            {
                Id = r.Id!,
                Timestamp = r.Timestamp!.Value,
                Name = r.Name,
                ColumnsMappingId = r.ColumnsMappingId,
                AccountType = r.AccountType,
            });
        }
        else
        {
            this.SetFailed();
        }
    }
}
