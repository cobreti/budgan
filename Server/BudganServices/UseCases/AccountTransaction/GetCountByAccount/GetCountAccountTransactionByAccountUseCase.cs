using BudganInfra.Repositories.AccountTransaction;

namespace BudganServices.UseCases.AccountTransaction.GetCountByAccount;

internal class GetCountAccountTransactionByAccountUseCase : BaseUseCaseWithResultValue<int?>, IGetCountAccountTransactionByAccountUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _accountId;

    public GetCountAccountTransactionByAccountUseCase(IAccountTransactionRepository accountTransactionRepository, Guid accountId)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountTransactionRepository.GetCountAccountTransactionByAccountRepoOperation(this._accountId);

        await repoOp.ExecuteAsync();

        this.SetSucceeded(repoOp.ResultValue.Count);
    }
}
