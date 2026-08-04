using BudganGlobal.Errors;
using BudganInfra.DBContext;
using BudganInfra.Repositories.AccountRecurringTransaction.Insert;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.ReplaceForAccount;

public class ReplaceAccountRecurringTransactionsForAccountRepoOp : BaseRepositoryOperation, IReplaceAccountRecurringTransactionsForAccountRepoOp
{
    private static readonly int[] UniqueConstraintViolationErrorNumbers = [2601, 2627];

    private readonly DataContext _dataContext;
    private readonly Guid _accountId;
    private readonly List<DaoInsertAccountRecurringTransaction> _items;

    public ReplaceAccountRecurringTransactionsForAccountRepoOp(
        DataContext dataContext,
        Guid accountId,
        List<DaoInsertAccountRecurringTransaction> items)
    {
        this._dataContext = dataContext;
        this._accountId = accountId;
        this._items = items;
    }

    public async Task ExecuteAsync()
    {
        var existing = await this._dataContext.AccountRecurringTransactions
            .Where(x => x.AccountId == this._accountId)
            .ToListAsync();

        if (existing.Count > 0)
        {
            this._dataContext.AccountRecurringTransactions.RemoveRange(existing);
        }

        if (this._items.Count > 0)
        {
            var entities = this._items.Select(x => new DBContext.Tables.AccountRecurringTransaction
            {
                Id = Guid.CreateVersion7(),
                AccountId = x.AccountId,
                RecurringId = x.RecurringId,
                PeriodInDays = x.PeriodInDays,
                TransactionCount = x.TransactionCount,
                Description = x.Description,
                AverageAmount = x.AverageAmount,
                FirstOccurrenceDate = x.FirstOccurrenceDate,
                LastOccurrenceDate = x.LastOccurrenceDate,
            });

            await this._dataContext.AccountRecurringTransactions.AddRangeAsync(entities);
        }

        try
        {
            await this._dataContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            this.SetFailed(BudganErrorValue.DuplicateAccountRecurringTransaction);
            return;
        }

        this.SetSucceeded();
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlException
               && sqlException.Errors.Cast<SqlError>().Any(e => UniqueConstraintViolationErrorNumbers.Contains(e.Number));
    }
}
