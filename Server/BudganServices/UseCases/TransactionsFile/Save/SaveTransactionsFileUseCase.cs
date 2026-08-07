using BudganInfra.Repositories.TransactionsFile;
using BudganInfra.Repositories.TransactionsFile.Save;

namespace BudganServices.UseCases.TransactionsFile.Save;

internal class SaveTransactionsFileUseCase : BaseUseCaseWithResultValue<Guid>, ISaveTransactionsFileUseCase
{
    private readonly ITransactionsFileRepository _transactionsFileRepository;
    private readonly BOSaveTransactionsFile _boSaveTransactionsFile;

    public SaveTransactionsFileUseCase(ITransactionsFileRepository transactionsFileRepository, BOSaveTransactionsFile model)
    {
        this._transactionsFileRepository = transactionsFileRepository;
        this._boSaveTransactionsFile = model;
    }

    public async Task ExecuteAsync()
    {
        var daoSave = new DaoSaveTransactionsFile
        {
            AccountId = this._boSaveTransactionsFile.AccountId,
            Content = this._boSaveTransactionsFile.Content,
            Filename = this._boSaveTransactionsFile.Filename,
            InsertionDate = this._boSaveTransactionsFile.InsertionDate,
        };

        var repoOp = this._transactionsFileRepository.SaveTransactionsFileRepoOperation(daoSave);

        await repoOp.ExecuteAsync();

        this.SetSucceeded(repoOp.ResultValue);
    }
}
