using BudganInfra.DBContext;
using BudganInfra.Repositories.AccountRecurringTransaction.DeleteByAccount;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.AccountRecurringTransaction.DeleteByAccount;

public class DeleteAccountRecurringTransactionByAccountRepoOpTests
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
            Description = "Subscription",
            AverageAmount = 10.00m,
            FirstOccurrenceDate = new DateOnly(2026, 1, 1),
            LastOccurrenceDate = new DateOnly(2026, 2, 1),
        };

        context.AccountRecurringTransactions.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    [Fact]
    public async Task Execute_DeletesAllRowsForAccount_LeavesOtherAccountsUntouched()
    {
        await using var context = CreateContext();
        var accountA = Guid.NewGuid();
        var accountB = Guid.NewGuid();

        await SeedRow(context, accountA, "recurring-a1");
        await SeedRow(context, accountA, "recurring-a2");
        var untouchedB = await SeedRow(context, accountB, "recurring-b1");

        var op = new DeleteAccountRecurringTransactionByAccountRepoOp(context, accountA);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        var remaining = await context.AccountRecurringTransactions.ToListAsync();
        Assert.Single(remaining);
        Assert.Equal(untouchedB.Id, remaining[0].Id);
    }

    [Fact]
    public async Task Execute_WhenAccountHasNoRows_Succeeds()
    {
        await using var context = CreateContext();
        var op = new DeleteAccountRecurringTransactionByAccountRepoOp(context, Guid.NewGuid());

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
    }
}
