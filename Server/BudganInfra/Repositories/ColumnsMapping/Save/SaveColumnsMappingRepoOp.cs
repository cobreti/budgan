using BudganGlobal.Errors;
using BudganInfra.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.ColumnsMapping.Save;

internal class SaveColumnsMappingRepoOp : BaseRepositoryOperationWithResultValue<Guid>, ISaveColumnsMappingRepoOp
{
    private static readonly int[] UniqueConstraintViolationErrorNumbers = { 2601, 2627 };

    private readonly DataContext _dataContext;
    private readonly DaoSaveColumnsMapping _daoSaveColumnsMapping;

    public DaoSaveColumnsMapping DaoSaveColumnsMapping => this._daoSaveColumnsMapping;


    public SaveColumnsMappingRepoOp(DataContext dataContext, DaoSaveColumnsMapping daoSaveColumnsMapping)
    {
        this._dataContext = dataContext;
        this._daoSaveColumnsMapping = daoSaveColumnsMapping;
    }

    public async Task ExecuteAsync()
    {
        var id = this._daoSaveColumnsMapping.Id != null ? Guid.Parse(this._daoSaveColumnsMapping.Id) : Guid.CreateVersion7();
        var columnsMapping = new DBContext.Entities.ColumnsMapping()
        {
            Id = id,
            Name = this._daoSaveColumnsMapping.Name,
            CardNumberColumnIndex = this._daoSaveColumnsMapping.CardNumberColumnIndex,
            CardNumberColumnText = this._daoSaveColumnsMapping.CardNumberColumnText,
            AmountColumnIndex = this._daoSaveColumnsMapping.AmountColumnIndex,
            AmountColumnText = this._daoSaveColumnsMapping.AmountColumnText,
            DateInscriptionColumnIndex = this._daoSaveColumnsMapping.DateInscriptionColumnIndex,
            DateInscriptionColumnText = this._daoSaveColumnsMapping.DateInscriptionColumnText,
            DescriptionColumnIndex = this._daoSaveColumnsMapping.DescriptionColumnIndex,
            DescriptionColumnText = this._daoSaveColumnsMapping.DescriptionColumnText,
        };

        await this._dataContext.ColumnsMappings.AddAsync(columnsMapping);

        if (!await this.TrySaveChanges())
        {
            return;
        }

        this.SetSucceeded(columnsMapping.Id);
    }

    private async Task<bool> TrySaveChanges()
    {
        try
        {
            await this._dataContext.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            this.SetFailed(BudganErrorValue.DuplicateColumnsMapping);
            return false;
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlException
               && sqlException.Errors.Cast<SqlError>().Any(e => UniqueConstraintViolationErrorNumbers.Contains(e.Number));
    }
}
