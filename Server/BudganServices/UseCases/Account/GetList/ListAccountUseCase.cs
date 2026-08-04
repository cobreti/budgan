using BudganInfra.Repositories.Account;

namespace BudganServices.UseCases.Account.GetList;

internal class ListAccountUseCase : BaseUseCaseWithResultValue<List<BOListAccount>>, IListAccountUseCase
{
    private readonly IAccountRepository _accountRepository;

    public ListAccountUseCase(IAccountRepository accountRepository)
    {
        this._accountRepository = accountRepository;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRepository.ListAccountRepoOperation();

        await repoOp.ExecuteAsync();

        var result = repoOp.ResultValue
            .Select(x => new BOListAccount
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                ColumnsMappingId = x.ColumnsMappingId,
                AccountType = x.AccountType,
            })
            .ToList();

        this.SetSucceeded(result);
    }
}
