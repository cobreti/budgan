using BudganInfra.Repositories.AccountRecurringTransaction;

namespace BudganServices.UseCases.AccountRecurringTransaction.DeleteByAccount;

internal class DeleteAccountRecurringTransactionByAccountUseCase : BaseUseCase, IDeleteAccountRecurringTransactionByAccountUseCase
{
    private readonly IAccountRecurringTransactionRepository _accountRecurringTransactionRepository;
    private readonly Guid _accountId;

    public DeleteAccountRecurringTransactionByAccountUseCase(IAccountRecurringTransactionRepository accountRecurringTransactionRepository, Guid accountId)
    {
        this._accountRecurringTransactionRepository = accountRecurringTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountRecurringTransactionRepository.DeleteAccountRecurringTransactionByAccountRepoOperation(this._accountId);

        await repoOp.ExecuteAsync();

        this.SetSucceeded();
    }
}
