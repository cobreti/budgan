using BudganInfra.DBContext;
using BudganInfra.Repositories.AccountRecurringTransaction.Insert;
using BudganInfra.Repositories.AccountRecurringTransaction.ReplaceForAccount;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.AccountRecurringTransaction.ReplaceForAccount;

public class ReplaceAccountRecurringTransactionsForAccountRepoOpTests
{
    private static DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private static async Task<BudganInfra.DBContext.Tables.AccountRecurringTransaction> SeedRow(
        DataContext context,
        Guid accountId,
        string recurringId)
    {
        var entity = new BudganInfra.DBContext.Tables.AccountRecurringTransaction
        {
            Id = Guid.CreateVersion7(),
            Timestamp = DateTime.UtcNow,
            AccountId = accountId,
            RecurringId = recurringId,
            PeriodInDays = 30,
            TransactionCount = 2,
            Description = "Old",
            AverageAmount = 10.00m,
            FirstOccurrenceDate = new DateOnly(2026, 1, 1),
            LastOccurrenceDate = new DateOnly(2026, 2, 1),
        };

        context.AccountRecurringTransactions.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    private static DaoInsertAccountRecurringTransaction CreateDao(Guid accountId, string recurringId)
    {
        return new DaoInsertAccountRecurringTransaction
        {
            AccountId = accountId,
            RecurringId = recurringId,
            PeriodInDays = 14.5,
            TransactionCount = 4,
            Description = "New",
            AverageAmount = 25.00m,
            FirstOccurrenceDate = new DateOnly(2026, 3, 1),
            LastOccurrenceDate = new DateOnly(2026, 6, 1),
        };
    }

    [Fact]
    public async Task Execute_ReplacesExistingRowsWithNewSet_InOneCall()
    {
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();
        await SeedRow(context, accountId, "old-1");
        await SeedRow(context, accountId, "old-2");

        var op = new ReplaceAccountRecurringTransactionsForAccountRepoOp(
            context,
            accountId,
            new List<DaoInsertAccountRecurringTransaction> { CreateDao(accountId, "new-1") });

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        var remaining = await context.AccountRecurringTransactions.Where(x => x.AccountId == accountId).ToListAsync();
        Assert.Single(remaining);
        Assert.Equal("new-1", remaining[0].RecurringId);
    }

    [Fact]
    public async Task Execute_CanReuseRecurringIdFromPreviousReplace_WithoutConflict()
    {
        // This is the core "one shot" guarantee: the old row sharing a RecurringId with a
        // new row must be removed before the new row is added, both within the same
        // SaveChangesAsync call, so reusing an id across replace calls never 409s.
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();
        await SeedRow(context, accountId, "shared-id");

        var op = new ReplaceAccountRecurringTransactionsForAccountRepoOp(
            context,
            accountId,
            new List<DaoInsertAccountRecurringTransaction> { CreateDao(accountId, "shared-id") });

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        var remaining = await context.AccountRecurringTransactions.Where(x => x.AccountId == accountId).ToListAsync();
        Assert.Single(remaining);
        Assert.Equal("New", remaining[0].Description);
    }

    [Fact]
    public async Task Execute_LeavesOtherAccountsUntouched()
    {
        await using var context = CreateContext();
        var accountA = Guid.NewGuid();
        var accountB = Guid.NewGuid();
        await SeedRow(context, accountA, "a-1");
        var untouchedB = await SeedRow(context, accountB, "b-1");

        var op = new ReplaceAccountRecurringTransactionsForAccountRepoOp(
            context,
            accountA,
            new List<DaoInsertAccountRecurringTransaction> { CreateDao(accountA, "a-2") });

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        var accountBRows = await context.AccountRecurringTransactions.Where(x => x.AccountId == accountB).ToListAsync();
        Assert.Single(accountBRows);
        Assert.Equal(untouchedB.Id, accountBRows[0].Id);
    }

    [Fact]
    public async Task Execute_WithEmptyItems_ClearsAccountWithoutError()
    {
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();
        await SeedRow(context, accountId, "old-1");

        var op = new ReplaceAccountRecurringTransactionsForAccountRepoOp(
            context,
            accountId,
            new List<DaoInsertAccountRecurringTransaction>());

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Empty(await context.AccountRecurringTransactions.Where(x => x.AccountId == accountId).ToListAsync());
    }

    [Fact]
    public async Task Execute_WithNoExistingRowsAndEmptyItems_Succeeds()
    {
        await using var context = CreateContext();

        var op = new ReplaceAccountRecurringTransactionsForAccountRepoOp(
            context,
            Guid.NewGuid(),
            new List<DaoInsertAccountRecurringTransaction>());

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
    }
}
