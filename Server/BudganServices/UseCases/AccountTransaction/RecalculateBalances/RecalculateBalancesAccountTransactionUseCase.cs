using BudganInfra.DBContext.Tables;
using BudganInfra.Repositories.AccountTransaction;
using BudganInfra.Repositories.AccountTransaction.GetListByAccount;
using BudganInfra.Repositories.AccountTransaction.UpdateBalances;

namespace BudganServices.UseCases.AccountTransaction.RecalculateBalances;

internal class RecalculateBalancesAccountTransactionUseCase : BaseUseCase, IRecalculateBalancesAccountTransactionUseCase
{
    private readonly IAccountTransactionRepository _accountTransactionRepository;
    private readonly Guid _accountId;

    public RecalculateBalancesAccountTransactionUseCase(IAccountTransactionRepository accountTransactionRepository, Guid accountId)
    {
        this._accountTransactionRepository = accountTransactionRepository;
        this._accountId = accountId;
    }

    public async Task ExecuteAsync()
    {
        var listOp = this._accountTransactionRepository.ListAccountTransactionByAccountRepoOperation(this._accountId);

        await listOp.ExecuteAsync();

        var all = listOp.ResultValue;

        var snapshot = all.FirstOrDefault(x => x.RecordType == AccountTransactionRecordType.Snapshot);
        var normal = all
            .Where(x => x.RecordType == AccountTransactionRecordType.Normal)
            .OrderBy(x => x.DateInscription)
            .ThenBy(x => x.UniqueKey, StringComparer.Ordinal)
            .ToList();

        var updates = snapshot == null
            ? BuildUpdatesWithoutSnapshot(normal)
            : BuildUpdatesWithSnapshot(normal, snapshot);

        var updateOp = this._accountTransactionRepository.UpdateBalancesAccountTransactionRepoOperation(updates);

        await updateOp.ExecuteAsync();

        if (updateOp.Succeeded)
        {
            this.SetSucceeded();
        }
        else
        {
            this.SetFailed();
        }
    }

    private static List<DaoAccountTransactionBalanceUpdate> BuildUpdatesWithoutSnapshot(List<DaoListAccountTransactionByAccount> normal)
    {
        decimal running = 0m;
        var offset = 0;

        return normal.Select(t =>
        {
            offset += 1;
            running += t.Amount;

            return new DaoAccountTransactionBalanceUpdate { Id = t.Id, Balance = running, BalanceDateOffset = offset };
        }).ToList();
    }

    private static List<DaoAccountTransactionBalanceUpdate> BuildUpdatesWithSnapshot(
        List<DaoListAccountTransactionByAccount> normal,
        DaoListAccountTransactionByAccount snapshot)
    {
        var before = normal.Where(t => t.DateInscription < snapshot.DateInscription).ToList();
        var afterOrEqual = normal.Where(t => t.DateInscription >= snapshot.DateInscription).ToList();

        var runningForward = snapshot.Amount;
        var offsetForward = 0;
        var updatedAfter = afterOrEqual.Select(t =>
        {
            offsetForward += 1;
            runningForward += t.Amount;

            return new DaoAccountTransactionBalanceUpdate { Id = t.Id, Balance = runningForward, BalanceDateOffset = offsetForward };
        }).ToList();

        var runningBackward = snapshot.Amount;
        var offsetBackward = 0;
        var updatedBefore = new List<DaoAccountTransactionBalanceUpdate>();
        for (var i = before.Count - 1; i >= 0; i--)
        {
            offsetBackward -= 1;
            var t = before[i];

            updatedBefore.Insert(0, new DaoAccountTransactionBalanceUpdate
            {
                Id = t.Id,
                Balance = runningBackward,
                BalanceDateOffset = offsetBackward,
            });

            runningBackward -= t.Amount;
        }

        return updatedBefore.Concat(updatedAfter).ToList();
    }
}
