using BudganInfra.Repositories.AccountTransaction;
using BudganServices.UseCases.AccountTransaction.RecalculateBalances;

namespace BudganServices.UseCases.AccountTransaction.Delete;

internal class DeleteAccountTransactionUseCase : BaseUseCaseWithResultValue<Guid>, IDeleteAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _id;

    public DeleteAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, Guid id)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var getOp = this._accountTransactionRepository.GetAccountTransactionRepoOperation(this._id);

        await getOp.ExecuteAsync();

        if (!getOp.Succeeded)
        {
            this.SetFailed();
            return;
        }

        var accountId = getOp.ResultValue.AccountId;

        var deleteOp = this._accountTransactionRepository.DeleteAccountTransactionRepoOperation(this._id);

        await deleteOp.ExecuteAsync();

        if (!deleteOp.Succeeded)
        {
            this.SetFailed();
            return;
        }

        var recalculateBalancesUseCase = new RecalculateBalancesAccountTransactionUseCase(this._accountTransactionRepository, accountId);
        await recalculateBalancesUseCase.ExecuteAsync();

        this.SetSucceeded(this._id);
    }
}
