using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.UpdateBalances;

public class UpdateBalancesAccountTransactionRepoOp : BaseRepositoryOperation, IUpdateBalancesAccountTransactionRepoOp
{
    private readonly DataContext _dataContext;
    private readonly List<DaoAccountTransactionBalanceUpdate> _updates;

    public UpdateBalancesAccountTransactionRepoOp(DataContext dataContext, List<DaoAccountTransactionBalanceUpdate> updates)
    {
        this._dataContext = dataContext;
        this._updates = updates;
    }

    public async Task ExecuteAsync()
    {
        if (this._updates.Count == 0)
        {
            this.SetSucceeded();
            return;
        }

        var ids = this._updates.Select(x => x.Id).ToList();

        var entities = await this._dataContext.AccountTransactions
            .Where(x => ids.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        foreach (var update in this._updates)
        {
            if (entities.TryGetValue(update.Id, out var entity))
            {
                entity.Balance = update.Balance;
                entity.BalanceDateOffset = update.BalanceDateOffset;
            }
        }

        await this._dataContext.SaveChangesAsync();

        this.SetSucceeded();
    }
}
