using BudganInfra.Repositories.AccountTransaction;
using BudganServices.UseCases.AccountTransaction.RecalculateBalances;

namespace BudganServices.UseCases.AccountTransaction.DeleteSnapshot;

internal class DeleteSnapshotAccountTransactionUseCase : BaseUseCase, IDeleteSnapshotAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _accountId;

    public DeleteSnapshotAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, Guid accountId)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var getOp = this._accountTransactionRepository.GetSnapshotAccountTransactionByAccountRepoOperation(this._accountId);

        await getOp.ExecuteAsync();

        if (getOp.Succeeded)
        {
            var deleteOp = this._accountTransactionRepository.DeleteAccountTransactionRepoOperation(Guid.Parse(getOp.ResultValue.Id!));
            await deleteOp.ExecuteAsync();
        }

        var recalculateBalancesUseCase = new RecalculateBalancesAccountTransactionUseCase(this._accountTransactionRepository, this._accountId);
        await recalculateBalancesUseCase.ExecuteAsync();

        this.SetSucceeded();
    }
}
