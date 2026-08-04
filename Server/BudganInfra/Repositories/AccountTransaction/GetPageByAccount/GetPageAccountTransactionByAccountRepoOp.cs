using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.GetPageByAccount;

public class GetPageAccountTransactionByAccountRepoOp
    : BaseRepositoryOperationWithResultValue<DaoGetPageAccountTransaction>, IGetPageAccountTransactionByAccountRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _accountId;
    private readonly int _page;
    private readonly int _pageSize;
    private readonly AccountTransactionSortField _sortField;
    private readonly SortDirection _sortDirection;

    public GetPageAccountTransactionByAccountRepoOp(
        DataContext dataContext,
        Guid accountId,
        int page,
        int pageSize,
        AccountTransactionSortField sortField,
        SortDirection sortDirection)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
        this._page = page;
        this._pageSize = pageSize;
        this._sortField = sortField;
        this._sortDirection = sortDirection;
    }

    public async Task ExecuteAsync()
    {
        var query = this._dataContext.AccountTransactions
            .Where(x => x.AccountId == this._accountId);

        var totalCount = await query.CountAsync();

        var items = await this.ApplySort(query)
            .Skip(this._page * this._pageSize)
            .Take(this._pageSize)
            .Select(x => new DaoPageItemAccountTransaction
            {
                Id = x.Id,
                AccountId = x.AccountId,
                UniqueKey = x.UniqueKey,
                RecurringId = x.RecurringId,
                FileId = x.FileId,
                CardNumber = x.CardNumber,
                DateInscription = x.DateInscription,
                Amount = x.Amount,
                Balance = x.Balance,
                BalanceDateOffset = x.BalanceDateOffset,
                Description = x.Description,
                RecordType = x.RecordType,
            })
            .ToListAsync();

        this.SetSucceeded(new DaoGetPageAccountTransaction
        {
            Items = items,
            TotalCount = totalCount,
        });
    }

    private IOrderedQueryable<DBContext.Tables.AccountTransaction> ApplySort(
        IQueryable<DBContext.Tables.AccountTransaction> query)
    {
        var ordered = (this._sortField, this._sortDirection) switch
        {
            (AccountTransactionSortField.CardNumber, SortDirection.Ascending) => query.OrderBy(x => x.CardNumber),
            (AccountTransactionSortField.CardNumber, SortDirection.Descending) => query.OrderByDescending(x => x.CardNumber),
            (AccountTransactionSortField.DateInscription, SortDirection.Ascending) => query.OrderBy(x => x.DateInscription),
            (AccountTransactionSortField.DateInscription, SortDirection.Descending) => query.OrderByDescending(x => x.DateInscription),
            (AccountTransactionSortField.Description, SortDirection.Ascending) => query.OrderBy(x => x.Description),
            (AccountTransactionSortField.Description, SortDirection.Descending) => query.OrderByDescending(x => x.Description),
            (AccountTransactionSortField.Amount, SortDirection.Ascending) => query.OrderBy(x => x.Amount),
            (AccountTransactionSortField.Amount, SortDirection.Descending) => query.OrderByDescending(x => x.Amount),
            _ => query.OrderBy(x => x.DateInscription),
        };

        return ordered.ThenBy(x => x.Id);
    }
}
