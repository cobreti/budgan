using BudganInfra.DBContext;
using BudganInfra.Repositories.TransactionsFile.Save;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.TransactionsFile.Save;

public class SaveTransactionsFileRepoOpTests
{
    private static DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private static DaoSaveTransactionsFile CreateDao(Guid accountId, string filename = "statement.csv")
    {
        return new DaoSaveTransactionsFile
        {
            AccountId = accountId,
            Content = "card,date,amount,description\n1234,2026-01-01,10.00,Coffee",
            Filename = filename,
            InsertionDate = new DateOnly(2026, 1, 1),
        };
    }

    [Fact]
    public async Task Execute_AddsNewRowAndSetsResultValue()
    {
        await using var context = CreateContext();
        var dao = CreateDao(Guid.NewGuid());
        var op = new SaveTransactionsFileRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.NotEqual(Guid.Empty, op.ResultValue);
        Assert.Single(context.TransactionsFiles);
    }

    [Fact]
    public async Task Execute_PersistsAllMappedFields()
    {
        await using var context = CreateContext();
        var dao = CreateDao(Guid.NewGuid());
        var op = new SaveTransactionsFileRepoOp(context, dao);

        await op.ExecuteAsync();

        var persisted = await context.TransactionsFiles.SingleAsync(x => x.Id == op.ResultValue);
        Assert.Equal(dao.AccountId, persisted.AccountId);
        Assert.Equal(dao.Content, persisted.Content);
        Assert.Equal(dao.Filename, persisted.Filename);
        Assert.Equal(dao.InsertionDate, persisted.InsertionDate);
    }

    [Fact]
    public async Task Execute_WithSameAccountIdAndFilenameAlreadyPersisted_DoesNotEnforceUniquenessUnderInMemoryProvider()
    {
        // Characterization test, not a spec: confirmed empirically that this project's EF
        // Core InMemory provider does NOT enforce HasIndex(...).IsUnique() constraints at
        // all, so the row is inserted without error. The graceful
        // SetFailed(DuplicateTransactionsFile) path — triggered by catching a
        // DbUpdateException wrapping a real SQL Server unique-violation (error 2601/2627)
        // — can only be exercised against a real SQL Server database, not this test suite.
        await using var context = CreateContext();
        var accountId = Guid.NewGuid();

        context.TransactionsFiles.Add(new BudganInfra.DBContext.Entities.TransactionsFile
        {
            Id = Guid.CreateVersion7(),
            AccountId = accountId,
            Content = "existing content",
            Filename = "statement.csv",
            InsertionDate = new DateOnly(2026, 1, 1),
        });
        await context.SaveChangesAsync();

        var op = new SaveTransactionsFileRepoOp(context, CreateDao(accountId, "statement.csv"));

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Equal(2, await context.TransactionsFiles.CountAsync());
    }
}
