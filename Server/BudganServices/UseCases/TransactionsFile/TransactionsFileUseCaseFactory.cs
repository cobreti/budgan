using BudganInfra.Repositories.TransactionsFile;
using BudganServices.UseCases.TransactionsFile.GetListByAccount;
using BudganServices.UseCases.TransactionsFile.Save;

namespace BudganServices.UseCases.TransactionsFile;

internal class TransactionsFileUseCaseFactory : ITransactionsFileUseCaseFactory
{
    private readonly ITransactionsFileRepository _transactionsFileRepository;

    public TransactionsFileUseCaseFactory(ITransactionsFileRepository transactionsFileRepository)
    {
        this._transactionsFileRepository = transactionsFileRepository;
    }

    public ISaveTransactionsFileUseCase SaveUseCase(BOSaveTransactionsFile model)
    {
        return new SaveTransactionsFileUseCase(this._transactionsFileRepository, model);
    }

    public IGetListByAccountTransactionsFileUseCase GetListByAccountUseCase(Guid accountId)
    {
        return new GetListByAccountTransactionsFileUseCase(this._transactionsFileRepository, accountId);
    }
}
