using BudganInfra.Repositories.TransactionsFile;

namespace BudganServices.UseCases.TransactionsFile.GetListByAccount;

internal class GetListByAccountTransactionsFileUseCase : BaseUseCaseWithResultValue<List<BOTransactionsFile>>, IGetListByAccountTransactionsFileUseCase
{
    private readonly ITransactionsFileRepository _transactionsFileRepository;
    private readonly Guid _accountId;

    public GetListByAccountTransactionsFileUseCase(ITransactionsFileRepository transactionsFileRepository, Guid accountId)
    {
        this._transactionsFileRepository = transactionsFileRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._transactionsFileRepository.GetListByAccountTransactionsFileRepoOperation(this._accountId);

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
