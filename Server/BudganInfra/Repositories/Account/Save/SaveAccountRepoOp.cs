using BudganGlobal.Errors;
using BudganInfra.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.Account.Save;

public class SaveAccountRepoOp : BaseRepositoryOperationWithResultValue<Guid>, ISaveAccountRepoOp
{
    private static readonly int[] UniqueConstraintViolationErrorNumbers = { 2601, 2627 };

    private readonly DataContext _context;
    private readonly DaoSaveAccount _daoSaveAccount;

    public SaveAccountRepoOp(DataContext context, DaoSaveAccount daoSaveAccount)
    {
        this._context = context;
        this._daoSaveAccount = daoSaveAccount;
    }

    public async Task ExecuteAsync()
    {
        var id = this._daoSaveAccount.Id != null ? Guid.Parse(this._daoSaveAccount.Id) : Guid.CreateVersion7();
        var accountEntity = new DBContext.Entities.Account
        {
            Id = id,
            Name = this._daoSaveAccount.Name,
            AccountType = this._daoSaveAccount.AccountType,
            ColumnsMappingId = this._daoSaveAccount.ColumnsMappingId,
        };

        await _context.AddAsync(accountEntity);

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
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex, out var violatedIndexName))
        {
            this.SetFailed(violatedIndexName == "IX_Account_Name" ? BudganErrorValue.DuplicateAccountName : BudganErrorValue.DuplicateAccount);
            return false;
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex, out string? violatedIndexName)
    {
        violatedIndexName = null;

        if (ex.InnerException is not SqlException sqlException)
        {
            return false;
        }

        var violation = sqlException.Errors.Cast<SqlError>()
            .FirstOrDefault(e => UniqueConstraintViolationErrorNumbers.Contains(e.Number));

        if (violation == null)
        {
            return false;
        }

        violatedIndexName = violation.Message.Contains("IX_Account_Name") ? "IX_Account_Name" : null;
        return true;
    }
}