using BudganGlobal.Errors;
using BudganInfra.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountRecurringTransaction.Insert;

public class InsertAccountRecurringTransactionRepoOp : BaseRepositoryOperation, IInsertAccountRecurringTransactionRepoOp
{
    private static readonly int[] UniqueConstraintViolationErrorNumbers = { 2601, 2627 };

    private readonly DataContext _dataContext;
    private readonly List<DaoInsertAccountRecurringTransaction> _items;

    public InsertAccountRecurringTransactionRepoOp(DataContext dataContext, List<DaoInsertAccountRecurringTransaction> items)
    {
        this._dataContext = dataContext;
        this._items = items;
    }

    public async Task ExecuteAsync()
    {
        if (this._items.Count == 0)
        {
            this.SetSucceeded();
            return;
        }

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
