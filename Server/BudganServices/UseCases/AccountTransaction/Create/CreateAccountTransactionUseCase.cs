using System.Globalization;
using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
using BudganInfra.DBContext.Tables;
using BudganInfra.Repositories.AccountTransaction;
using BudganInfra.Repositories.AccountTransaction.Save;
using BudganServices.UseCases.AccountTransaction.RecalculateBalances;

namespace BudganServices.UseCases.AccountTransaction.Create;

internal class CreateAccountTransactionUseCase : BaseUseCaseWithResultValue<Guid>, ICreateAccountTransactionUseCase
{
    private const decimal RecurringRange = 0.15m;

    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly BOCreateAccountTransaction _model;

    public CreateAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, BOCreateAccountTransaction model)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._model = model;
    }

    public async Task ExecuteAsync()
    {
        var dateInscription = DateOnly.ParseExact(this._model.DateInscriptionAsString, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        var uniqueKey = BuildUniqueKey(this._model, this._model.DateInscriptionAsString);
        var recurringId = BuildRecurringId(this._model);

        var dao = new DaoSaveAccountTransaction
        {
            Id = null,
            Timestamp = null,
            AccountId = this._model.AccountId,
            UniqueKey = uniqueKey,
            RecurringId = recurringId,
            FileId = this._model.FileId,
            CardNumber = this._model.CardNumber,
            DateInscription = dateInscription,
            Amount = this._model.Amount,
            Balance = null,
            BalanceDateOffset = null,
            Description = this._model.Description,
            RecordType = AccountTransactionRecordType.Normal,
        };

        var repoOp = this._accountTransactionRepository.SaveAccountTransactionRepoOperation(dao);

        await repoOp.ExecuteAsync();

        if (!repoOp.Succeeded)
        {
            // SaveAccountTransactionRepoOp's only non-throwing failure path is a duplicate
            // unique-key violation; other failure conditions throw directly from the repo op.
            throw new BudganException(BudganErrorValue.DuplicateAccountTransaction);
        }

        var recalculateBalancesUseCase = new RecalculateBalancesAccountTransactionUseCase(this._accountTransactionRepository, this._model.AccountId);
        await recalculateBalancesUseCase.ExecuteAsync();

        this.SetSucceeded(repoOp.ResultValue);
    }

    private static string BuildUniqueKey(BOCreateAccountTransaction model, string dateInscriptionAsString)
    {
        return $"{model.AccountId}|{model.CardNumber}|{dateInscriptionAsString}|{model.Amount.ToString(CultureInfo.InvariantCulture)}|{model.Description}";
    }

    private static string BuildRecurringId(BOCreateAccountTransaction model)
    {
        var lower = (long)(Math.Floor(model.Amount * (1 - RecurringRange) / 5m) * 5m);
        var upper = (long)(Math.Floor(model.Amount * (1 + RecurringRange) / 5m) * 5m);

        return $"{model.AccountId}|{model.CardNumber}|{lower}|{upper}|{model.Description}";
    }
}
