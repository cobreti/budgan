using BudganInfra.DBContext;
using BudganInfra.Repositories.AccountRecurringTransaction.Insert;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.AccountRecurringTransaction.Insert;

public class InsertAccountRecurringTransactionRepoOpTests
{
    private static DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private static DaoInsertAccountRecurringTransaction CreateDao(Guid accountId, string recurringId)
    {
        return new DaoInsertAccountRecurringTransaction
        {
            AccountId = accountId,
            RecurringId = recurringId,
            PeriodInDays = 30.5,
            TransactionCount = 3,
            Description = "Gym membership",
            AverageAmount = 42.50m,
            FirstOccurrenceDate = new DateOnly(2026, 1, 1),
            LastOccurrenceDate = new DateOnly(2026, 3, 1),
        };
    }

    [Fact]
    public async Task Execute_WithItems_AddsAllRowsWithDistinctGeneratedIds()
    {
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();
        var items = new List<DaoInsertAccountRecurringTransaction>
        {
            CreateDao(accountId, "recurring-1"),
            CreateDao(accountId, "recurring-2"),
        };

        var op = new InsertAccountRecurringTransactionRepoOp(context, items);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        var persisted = await context.AccountRecurringTransactions.ToListAsync();
        Assert.Equal(2, persisted.Count);
        Assert.NotEqual(persisted[0].Id, persisted[1].Id);
        Assert.All(persisted, x => Assert.NotEqual(Guid.Empty, x.Id));
    }

    [Fact]
    public async Task Execute_WithItems_PersistsAllMappedFields()
    {
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();
        var dao = CreateDao(accountId, "recurring-1");

        var op = new InsertAccountRecurringTransactionRepoOp(context, new List<DaoInsertAccountRecurringTransaction> { dao });

        await op.ExecuteAsync();

        var persisted = await context.AccountRecurringTransactions.SingleAsync();
        Assert.Equal(dao.AccountId, persisted.AccountId);
        Assert.Equal(dao.RecurringId, persisted.RecurringId);
        Assert.Equal(dao.PeriodInDays, persisted.PeriodInDays);
        Assert.Equal(dao.TransactionCount, persisted.TransactionCount);
        Assert.Equal(dao.Description, persisted.Description);
        Assert.Equal(dao.AverageAmount, persisted.AverageAmount);
        Assert.Equal(dao.FirstOccurrenceDate, persisted.FirstOccurrenceDate);
        Assert.Equal(dao.LastOccurrenceDate, persisted.LastOccurrenceDate);
    }

    [Fact]
    public async Task Execute_WithEmptyList_SucceedsWithoutAddingRows()
    {
        await using var context = CreateContext();
        var op = new InsertAccountRecurringTransactionRepoOp(context, new List<DaoInsertAccountRecurringTransaction>());

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Empty(await context.AccountRecurringTransactions.ToListAsync());
    }

    [Fact]
    public async Task Execute_WithRecurringIdAlreadyPersisted_DoesNotEnforceUniquenessUnderInMemoryProvider()
    {
        // Characterization test, not a spec: confirmed empirically that this project's EF
        // Core InMemory provider does NOT enforce HasIndex(...).IsUnique() constraints at
        // all (neither within a single AddRange/SaveChanges batch nor against an
        // already-persisted row), so the row is inserted without error. The graceful
        // SetFailed(DuplicateAccountRecurringTransaction) path — triggered by catching a
        // DbUpdateException wrapping a real SQL Server unique-violation (error 2601/2627)
        // — can only be exercised against a real SQL Server database, not this test suite.
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();

        context.AccountRecurringTransactions.Add(new BudganInfra.DBContext.Tables.AccountRecurringTransaction
        {
            Id = Guid.CreateVersion7(),
            Timestamp = DateTime.UtcNow,
            AccountId = accountId,
            RecurringId = "same-recurring-id",
            PeriodInDays = 30,
            TransactionCount = 1,
            Description = "Existing",
            AverageAmount = 5.00m,
            FirstOccurrenceDate = new DateOnly(2026, 1, 1),
            LastOccurrenceDate = new DateOnly(2026, 1, 1),
        });
        await context.SaveChangesAsync();

        var op = new InsertAccountRecurringTransactionRepoOp(
            context,
            new List<DaoInsertAccountRecurringTransaction> { CreateDao(accountId, "same-recurring-id") });

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Equal(2, await context.AccountRecurringTransactions.CountAsync());
    }
}
