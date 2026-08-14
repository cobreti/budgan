using BudganInfra.DBContext;
using BudganInfra.Repositories.ColumnsMapping.Save;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.ColumnsMapping.Save;

public class SaveColumnsMappingRepoOpTests
{
    private static DataContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new DataContext(options);
    }

    private static DaoSaveColumnsMapping CreateDao(string? id = null, DateTime? timestamp = null)
    {
        return new DaoSaveColumnsMapping
        {
            Id = id,
            Timestamp = timestamp,
            Name = "My Mapping",
            CardNumberColumnIndex = 0,
            CardNumberColumnText = "Card Number",
            AmountColumnIndex = 1,
            AmountColumnText = "Amount",
            DateInscriptionColumnIndex = 2,
            DateInscriptionColumnText = "Date",
            DescriptionColumnIndex = 3,
            DescriptionColumnText = "Description",
            DateFormat = @"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})",
        };
    }

    private static async Task<BudganInfra.DBContext.Entities.ColumnsMapping> SeedColumnsMapping(DataContext context, DateTime timestamp)
    {
        var entity = new BudganInfra.DBContext.Entities.ColumnsMapping
        {
            Id = Guid.CreateVersion7(),
            Timestamp = timestamp,
            Name = "Original Mapping",
            CardNumberColumnIndex = 10,
            CardNumberColumnText = "Original Card Number",
            AmountColumnIndex = 11,
            AmountColumnText = "Original Amount",
            DateInscriptionColumnIndex = 12,
            DateInscriptionColumnText = "Original Date",
            DescriptionColumnIndex = 13,
            DescriptionColumnText = "Original Description",
            DateFormat = @"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})",
        };

        context.ColumnsMappings.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    [Fact]
    public async Task Execute_WhenIdIsNull_AddsNewRowAndSetsSaveResultValue()
    {
        await using var context = CreateContext();
        var dao = CreateDao();
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.NotEqual(Guid.Empty, op.ResultValue);
        Assert.True(op.Succeeded);
        Assert.Single(context.ColumnsMappings);
    }

    [Fact]
    public async Task Execute_WhenIdIsNull_PersistsAllMappedFields()
    {
        await using var context = CreateContext();
        var dao = CreateDao();
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await op.ExecuteAsync();

        var persisted = await context.ColumnsMappings.SingleAsync(x => x.Id == op.ResultValue);
        Assert.Equal(dao.Name, persisted.Name);
        Assert.Equal(dao.CardNumberColumnIndex, persisted.CardNumberColumnIndex);
        Assert.Equal(dao.CardNumberColumnText, persisted.CardNumberColumnText);
        Assert.Equal(dao.AmountColumnIndex, persisted.AmountColumnIndex);
        Assert.Equal(dao.AmountColumnText, persisted.AmountColumnText);
        Assert.Equal(dao.DateInscriptionColumnIndex, persisted.DateInscriptionColumnIndex);
        Assert.Equal(dao.DateInscriptionColumnText, persisted.DateInscriptionColumnText);
        Assert.Equal(dao.DescriptionColumnIndex, persisted.DescriptionColumnIndex);
        Assert.Equal(dao.DescriptionColumnText, persisted.DescriptionColumnText);
        Assert.Equal(dao.DateFormat, persisted.DateFormat);
    }

    [Fact]
    public async Task Execute_WhenIdIsGivenAndDoesNotExist_InsertsRowWithGivenId()
    {
        await using var context = CreateContext();
        var givenId = Guid.NewGuid();
        var dao = CreateDao(id: givenId.ToString());
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Equal(givenId, op.ResultValue);
        Assert.Single(context.ColumnsMappings);

        var persisted = await context.ColumnsMappings.SingleAsync(x => x.Id == givenId);
        Assert.Equal(dao.Name, persisted.Name);
    }

    [Fact]
    public async Task Execute_WhenIdAlreadyExists_ThrowsUnderInMemoryProviderInsteadOfSettingFailed()
    {
        // Characterization test, not a spec: because Id is the primary key, EF Core's
        // identity map rejects tracking a second entity with the same key inside the same
        // DbContext, raising InvalidOperationException from AddAsync() itself — before
        // SaveChangesAsync() is ever reached. The graceful SetFailed(DuplicateColumnsMapping)
        // path — triggered by catching a DbUpdateException wrapping a real SQL Server
        // primary-key violation (error 2601/2627) on a fresh, per-request DbContext — can
        // only be exercised against a real SQL Server database, not this test suite (mirrors
        // SaveTransactionsFileRepoOpTests' equivalent characterization test).
        await using var context = CreateContext();
        var seeded = await SeedColumnsMapping(context, DateTime.UtcNow);

        var dao = CreateDao(id: seeded.Id.ToString());
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await Assert.ThrowsAsync<InvalidOperationException>(() => op.ExecuteAsync());
    }

    [Theory]
    [InlineData("not-a-guid")]
    [InlineData("")]
    public async Task Execute_WhenIdIsNotAValidGuid_ThrowsFormatException(string invalidId)
    {
        await using var context = CreateContext();
        var dao = CreateDao(id: invalidId, timestamp: DateTime.UtcNow);
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await Assert.ThrowsAsync<FormatException>(() => op.ExecuteAsync());
    }

    [Fact]
    public async Task ErrorValue_Get_AlwaysThrowsInvalidOperationException()
    {
        // ErrorValue is only populated on a duplicate-id failure; a plain insert (no id given)
        // always succeeds, so ErrorValue stays unreachable here.
        await using var context = CreateContext();
        var dao = CreateDao();
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.Throws<InvalidOperationException>(() => op.BudganErrorValue);
    }
}
