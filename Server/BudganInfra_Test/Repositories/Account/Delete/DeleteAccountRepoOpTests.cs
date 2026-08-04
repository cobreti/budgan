using BudganInfra.DBContext;
using BudganInfra.Repositories.Account.Delete;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.Account.Delete;

public class DeleteAccountRepoOpTests
{
    private static DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private static async Task<BudganInfra.DBContext.Tables.Account> SeedAccount(
        DataContext context,
        string name,
        string accountType)
    {
        var entity = new BudganInfra.DBContext.Tables.Account
        {
            Id = Guid.CreateVersion7(),
            Timestamp = DateTime.UtcNow,
            Name = name,
            AccountType = accountType,
            ColumnsMappingId = Guid.NewGuid(),
        };

        context.Accounts.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    [Fact]
    public async Task Execute_WhenAccountExists_RemovesEntityAndSetsResultValue()
    {
        await using var context = CreateContext();
        var seeded = await SeedAccount(context, "Checking", "Checking");

        var op = new DeleteAccountRepoOp(context, seeded.Id);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Equal(seeded.Id, op.ResultValue);
        Assert.False(await context.Accounts.AnyAsync(x => x.Id == seeded.Id));
    }

    [Fact]
    public async Task Execute_WhenAccountDoesNotExist_SetsFailed()
    {
        await using var context = CreateContext();
        var op = new DeleteAccountRepoOp(context, Guid.NewGuid());

        await op.ExecuteAsync();

        Assert.False(op.Succeeded);
    }
}
