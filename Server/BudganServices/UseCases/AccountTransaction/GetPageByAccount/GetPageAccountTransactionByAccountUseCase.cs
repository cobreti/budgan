using System.Globalization;
using BudganInfra.Repositories.AccountTransaction;
using InfraSortField = BudganInfra.Repositories.AccountTransaction.GetPageByAccount.AccountTransactionSortField;
using InfraSortDirection = BudganInfra.Repositories.AccountTransaction.GetPageByAccount.SortDirection;

namespace BudganServices.UseCases.AccountTransaction.GetPageByAccount;

internal class GetPageAccountTransactionByAccountUseCase : BaseUseCaseWithResultValue<BOGetPageAccountTransaction>, IGetPageAccountTransactionByAccountUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _accountId;
    private readonly int _page;
    private readonly int _pageSize;
    private readonly InfraSortField _sortField;
    private readonly InfraSortDirection _sortDirection;

    public GetPageAccountTransactionByAccountUseCase(
        IAccountTransactionRepository accountTransactionRepository,
        Guid accountId,
        int page,
        int pageSize,
        InfraSortField sortField,
        InfraSortDirection sortDirection)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._accountId = accountId;
        this._page = page;
        this._pageSize = pageSize;
        this._sortField = sortField;
        this._sortDirection = sortDirection;
    }

    public async Task ExecuteAsync()
    {
        var repoOp = this._accountTransactionRepository.GetPageAccountTransactionByAccountRepoOperation(
            this._accountId,
            this._page,
            this._pageSize,
            this._sortField,
            this._sortDirection);

        await repoOp.ExecuteAsync();

        var r = repoOp.ResultValue;

        this.SetSucceeded(new BOGetPageAccountTransaction
        {
            TotalCount = r.TotalCount,
            Items = r.Items
                .Select(x => new BOAccountTransactionPageItem
                {
                    Id = x.Id.ToString(),
                    AccountId = x.AccountId,
                    FileId = x.FileId,
                    CardNumber = x.CardNumber,
                    DateInscriptionAsString = x.DateInscription.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Amount = x.Amount,
                    Balance = x.Balance,
                    BalanceDateOffset = x.BalanceDateOffset,
                    Description = x.Description,
                    UniqueKey = x.UniqueKey,
                    RecurringId = x.RecurringId,
                    RecordType = x.RecordType,
                })
                .ToList(),
        });
    }
}
