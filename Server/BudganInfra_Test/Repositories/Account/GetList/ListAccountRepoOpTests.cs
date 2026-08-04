using BudganInfra.DBContext;
using BudganInfra.Repositories.Account.GetList;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra_Test.Repositories.Account.GetList;

public class ListAccountRepoOpTests
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
            Name = name,
            AccountType = accountType,
            ColumnsMappingId = Guid.NewGuid(),
        };

        context.Accounts.Add(entity);
        await context.SaveChangesAsync();

        return entity;
    }

    [Fact]
    public async Task Execute_WhenAccountsExist_ReturnsAllMappedToDao()
    {
        await using var context = CreateContext();
        var first = await SeedAccount(context, "Checking", "Checking");
        var second = await SeedAccount(context, "Savings", "Savings");

        var op = new ListAccountRepoOp(context);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Equal(2, op.ResultValue.Count);

        var firstDao = op.ResultValue.Single(x => x.Id == first.Id);
        Assert.Equal(first.Name, firstDao.Name);
        Assert.Equal(first.ColumnsMappingId, firstDao.ColumnsMappingId);
        Assert.Equal(first.AccountType, firstDao.AccountType);

        var secondDao = op.ResultValue.Single(x => x.Id == second.Id);
        Assert.Equal(second.Name, secondDao.Name);
        Assert.Equal(second.ColumnsMappingId, secondDao.ColumnsMappingId);
        Assert.Equal(second.AccountType, secondDao.AccountType);
    }

    [Fact]
    public async Task Execute_WhenNoAccountsExist_ReturnsEmptyList()
    {
        await using var context = CreateContext();
        var op = new ListAccountRepoOp(context);

        await op.ExecuteAsync();

        Assert.True(op.Succeeded);
        Assert.Empty(op.ResultValue);
    }
}
