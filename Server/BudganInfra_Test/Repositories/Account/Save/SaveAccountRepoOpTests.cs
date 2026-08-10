using BudganInfra.DBContext;
using BudganInfra.Repositories.Account.Save;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.Account.Save;

public class SaveAccountRepoOpTests
{
    private static DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private static DaoSaveAccount CreateDao(string name = "Checking")
    {
        return new DaoSaveAccount
        {
            Name = name,
            ColumnsMappingId = Guid.NewGuid(),
            AccountType = "Checking",
        };
    }

    [Fact]
    public async Task Execute_AddsNewRowAndSetsResultValue()
    {
        await using var context = CreateContext();
        var dao = CreateDao();
        var op = new SaveAccountRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.NotEqual(Guid.Empty, op.ResultValue);
        Assert.Single(context.Accounts);
    }

    [Fact]
    public async Task Execute_PersistsAllMappedFields()
    {
        await using var context = CreateContext();
        var dao = CreateDao();
        var op = new SaveAccountRepoOp(context, dao);

        await op.ExecuteAsync();

        var persisted = await context.Accounts.SingleAsync(x => x.Id == op.ResultValue);
        Assert.Equal(dao.Name, persisted.Name);
        Assert.Equal(dao.ColumnsMappingId, persisted.ColumnsMappingId);
        Assert.Equal(dao.AccountType, persisted.AccountType);
    }

    [Fact]
    public async Task Execute_WithSameNameAlreadyPersisted_DoesNotEnforceUniquenessUnderInMemoryProvider()
    {
        // Characterization test, not a spec: confirmed empirically that this project's EF
        // Core InMemory provider does NOT enforce HasIndex(...).IsUnique() constraints at
        // all, so the row is inserted without error. The graceful
        // SetFailed(DuplicateAccountName) path — triggered by catching a DbUpdateException
        // wrapping a real SQL Server unique-violation (error 2601/2627) on the IX_Account_Name
        // index — can only be exercised against a real SQL Server database, not this test suite.
        await using var context = CreateContext();

        context.Accounts.Add(new BudganInfra.DBContext.Entities.Account
        {
            Id = Guid.CreateVersion7(),
            Name = "Checking",
            AccountType = "Checking",
            ColumnsMappingId = Guid.NewGuid(),
        });
        await context.SaveChangesAsync();

        var op = new SaveAccountRepoOp(context, CreateDao("Checking"));

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Equal(2, await context.Accounts.CountAsync());
    }
}
