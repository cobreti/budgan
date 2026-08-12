using BudganInfra.Repositories.TransactionsFile;

namespace BudganServices.UseCases.TransactionsFile.GetList;

internal class GetListTransactionsFileUseCase : BaseUseCaseWithResultValue<List<BOTransactionsFile>>, IGetListTransactionsFileUseCase
{
    private readonly ITransactionsFileRepository _transactionsFileRepository;

    public GetListTransactionsFileUseCase(ITransactionsFileRepository transactionsFileRepository)
    {
        this._transactionsFileRepository = transactionsFileRepository;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._transactionsFileRepository.GetListTransactionsFileRepoOperation();

        await repoOp.ExecuteAsync();

        var result = repoOp.ResultValue
            .Select(x => new BOTransactionsFile
            {
                Id = x.Id,
                AccountId = x.AccountId,
                Content = x.Content,
                Filename = x.Filename,
                InsertionDate = x.InsertionDate,
            })
            .ToList();

        this.SetSucceeded(result);
    }
}
