using System.Globalization;
using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganInfra.DBContext.Tables;
using BudganInfra.Repositories.AccountTransaction;
using BudganInfra.Repositories.AccountTransaction.Save;
using BudganServices.UseCases.AccountTransaction.RecalculateBalances;

namespace BudganServices.UseCases.AccountTransaction.SetSnapshot;

internal class SetSnapshotAccountTransactionUseCase : BaseUseCaseWithResultValue<Guid>, ISetSnapshotAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly BOSetAccountTransactionSnapshot _model;

    public SetSnapshotAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, BOSetAccountTransactionSnapshot model)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._model = model;
    }

    public async Task ExecuteAsync()
    {
        var getOp = this._accountTransactionRepository.GetSnapshotAccountTransactionByAccountRepoOperation(this._model.AccountId);

        await getOp.ExecuteAsync();

        var uniqueKey = $"snapshot|{this._model.AccountId}";
        var dateInscription = DateOnly.ParseExact(this._model.DateAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var dao = new DaoSaveAccountTransaction
        {
            Id = getOp.Succeeded ? getOp.ResultValue.Id : null,
            Timestamp = getOp.Succeeded ? getOp.ResultValue.Timestamp : null,
            AccountId = this._model.AccountId,
            UniqueKey = uniqueKey,
            RecurringId = uniqueKey,
            FileId = null,
            CardNumber = string.Empty,
            DateInscription = dateInscription,
            Amount = this._model.Amount,
            Balance = this._model.Amount,
            BalanceDateOffset = 0,
            Description = string.Empty,
            RecordType = AccountTransactionRecordType.Snapshot,
        };

        var saveOp = this._accountTransactionRepository.SaveAccountTransactionRepoOperation(dao);

        await saveOp.ExecuteAsync();

        if (!saveOp.Succeeded)
        {
            // SaveAccountTransactionRepoOp's only non-throwing failure path is a duplicate
            // unique-key violation; other failure conditions throw directly from the repo op.
            throw new BudganException(BudganErrorValue.DuplicateAccountTransaction);
        }

        var recalculateBalancesUseCase = new RecalculateBalancesAccountTransactionUseCase(this._accountTransactionRepository, this._model.AccountId);
        await recalculateBalancesUseCase.ExecuteAsync();

        this.SetSucceeded(saveOp.ResultValue);
    }
}
