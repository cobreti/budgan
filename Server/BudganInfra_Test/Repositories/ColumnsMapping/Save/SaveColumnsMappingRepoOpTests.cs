using BudganGlobal.Errors;
using BudganGlobal.Errors.Exceptions;
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
        };
    }

    private static async Task<BudganInfra.DBContext.Tables.ColumnsMapping> SeedColumnsMapping(DataContext context, DateTime timestamp)
    {
        var entity = new BudganInfra.DBContext.Tables.ColumnsMapping
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
    }

    [Fact]
    public async Task Execute_WhenIdMatchesAndTimestampMatches_UpdatesExistingRowAndSetsSaveResultValue()
    {
        await using var context = CreateContext();
        var timestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var seeded = await SeedColumnsMapping(context, timestamp);

        var dao = CreateDao(id: seeded.Id.ToString(), timestamp: timestamp);
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.Equal(seeded.Id, op.ResultValue);
        Assert.Single(context.ColumnsMappings);

        var persisted = await context.ColumnsMappings.SingleAsync(x => x.Id == seeded.Id);
        Assert.Equal(dao.Name, persisted.Name);
        Assert.Equal(dao.CardNumberColumnIndex, persisted.CardNumberColumnIndex);
        Assert.Equal(dao.AmountColumnIndex, persisted.AmountColumnIndex);
        Assert.Equal(dao.DateInscriptionColumnIndex, persisted.DateInscriptionColumnIndex);
        Assert.Equal(dao.DescriptionColumnIndex, persisted.DescriptionColumnIndex);
    }

    [Fact]
    public async Task Execute_WhenIdDoesNotExist_ThrowsBudganExceptionWithResourceNotFound()
    {
        await using var context = CreateContext();
        var dao = CreateDao(id: Guid.NewGuid().ToString(), timestamp: DateTime.UtcNow);
        var op = new SaveColumnsMappingRepoOp(context, dao);

        var ex = await Assert.ThrowsAsync<BudganException>(() => op.ExecuteAsync());

        Assert.Equal(BudganErrorValue.ResourceNotFound, ex.BudganError);
    }

    [Fact]
    public async Task Execute_WhenUpdateTimestampIsNull_ThrowsException()
    {
        await using var context = CreateContext();
        var seeded = await SeedColumnsMapping(context, DateTime.UtcNow);

        var dao = CreateDao(id: seeded.Id.ToString(), timestamp: null);
        var op = new SaveColumnsMappingRepoOp(context, dao);

        var ex = await Assert.ThrowsAsync<Exception>(() => op.ExecuteAsync());

        Assert.Equal("timestamp value required for update operation", ex.Message);
    }

    [Fact]
    public async Task Execute_WhenUpdateTimestampDoesNotMatch_ThrowsException()
    {
        await using var context = CreateContext();
        var seeded = await SeedColumnsMapping(context, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var dao = CreateDao(id: seeded.Id.ToString(), timestamp: new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc));
        var op = new SaveColumnsMappingRepoOp(context, dao);

        var ex = await Assert.ThrowsAsync<Exception>(() => op.ExecuteAsync());

        Assert.Equal("indicated resource has been modified", ex.Message);
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
        // Characterization test: Succeeded is hardcoded true in the current implementation
        // and never flipped to false, so ErrorValue is unreachable after any successful Execute().
        await using var context = CreateContext();
        var dao = CreateDao();
        var op = new SaveColumnsMappingRepoOp(context, dao);

        await op.ExecuteAsync();

        Assert.Throws<InvalidOperationException>(() => op.BudganErrorValue);
    }
}
