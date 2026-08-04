using BudganGlobal.Errors;
using BudganInfra.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.Save;

public class SaveAccountTransactionRepoOp : BaseRepositoryOperationWithResultValue<Guid>, ISaveAccountTransactionRepoOp
{
    private static readonly int[] UniqueConstraintViolationErrorNumbers = { 2601, 2627 };

    private readonly DataContext _context;
    private readonly DaoSaveAccountTransaction _daoSaveAccountTransaction;

    public SaveAccountTransactionRepoOp(DataContext context, DaoSaveAccountTransaction daoSaveAccountTransaction)
    {
        this._context = context;
        this._daoSaveAccountTransaction = daoSaveAccountTransaction;
    }

    public async Task ExecuteAsync()
    {
        if (this._daoSaveAccountTransaction.Id == null)
        {
            await this.Add();
        }
        else
        {
            await this.Update();
        }
    }

    private async Task Add()
    {
        var id = Guid.CreateVersion7();
        var entity = new DBContext.Tables.AccountTransaction
        {
            Id = id,
            AccountId = this._daoSaveAccountTransaction.AccountId,
            UniqueKey = this._daoSaveAccountTransaction.UniqueKey,
            RecurringId = this._daoSaveAccountTransaction.RecurringId,
            FileId = this._daoSaveAccountTransaction.FileId,
            CardNumber = this._daoSaveAccountTransaction.CardNumber,
            DateInscription = this._daoSaveAccountTransaction.DateInscription,
            Amount = this._daoSaveAccountTransaction.Amount,
            Balance = this._daoSaveAccountTransaction.Balance,
            BalanceDateOffset = this._daoSaveAccountTransaction.BalanceDateOffset,
            Description = this._daoSaveAccountTransaction.Description,
            RecordType = this._daoSaveAccountTransaction.RecordType,
        };

        await this._context.AddAsync(entity);

        if (!await this.TrySaveChanges())
        {
            return;
        }

        this.SetSucceeded(id);
    }

    private async Task Update()
    {
        ArgumentNullException.ThrowIfNull(this._daoSaveAccountTransaction.Id);

        var id = Guid.Parse(this._daoSaveAccountTransaction.Id);
        var entity = await this._context.AccountTransactions.FirstOrDefaultAsync(x => x.Id == id);

        ValidateCanPerformUpdate(entity, this._daoSaveAccountTransaction);

        entity.AccountId = this._daoSaveAccountTransaction.AccountId;
        entity.UniqueKey = this._daoSaveAccountTransaction.UniqueKey;
        entity.RecurringId = this._daoSaveAccountTransaction.RecurringId;
        entity.FileId = this._daoSaveAccountTransaction.FileId;
        entity.CardNumber = this._daoSaveAccountTransaction.CardNumber;
        entity.DateInscription = this._daoSaveAccountTransaction.DateInscription;
        entity.Amount = this._daoSaveAccountTransaction.Amount;
        entity.Balance = this._daoSaveAccountTransaction.Balance;
        entity.BalanceDateOffset = this._daoSaveAccountTransaction.BalanceDateOffset;
        entity.Description = this._daoSaveAccountTransaction.Description;
        entity.RecordType = this._daoSaveAccountTransaction.RecordType;

        this._context.Update(entity);

        if (!await this.TrySaveChanges())
        {
            return;
        }

        this.SetSucceeded(id);
    }

    private async Task<bool> TrySaveChanges()
    {
        try
        {
            await this._context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            this.SetFailed(BudganErrorValue.DuplicateAccountTransaction);
            return false;
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlException
               && sqlException.Errors.Cast<SqlError>().Any(e => UniqueConstraintViolationErrorNumbers.Contains(e.Number));
    }
}
