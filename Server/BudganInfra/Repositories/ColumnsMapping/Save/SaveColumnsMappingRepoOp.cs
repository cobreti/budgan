using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.ColumnsMapping.Save;

internal class SaveColumnsMappingRepoOp : BaseRepositoryOperationWithResultValue<Guid>, ISaveColumnsMappingRepoOp
{
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
        if (this._daoSaveColumnsMapping.Id == null)
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
        var columnsMapping = new DBContext.Tables.ColumnsMapping()
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
        await this._dataContext.SaveChangesAsync();

        this.SetSucceeded(columnsMapping.Id);
    }

    private async Task Update()
    {
        ArgumentNullException.ThrowIfNull(this._daoSaveColumnsMapping.Id);

        var id = Guid.Parse(this._daoSaveColumnsMapping.Id);
        var columnsMapping = await this._dataContext.ColumnsMappings
            .FirstOrDefaultAsync(x => x.Id == id);

        ValidateCanPerformUpdate(columnsMapping, this._daoSaveColumnsMapping);

        columnsMapping.Name = this._daoSaveColumnsMapping.Name;
        columnsMapping.CardNumberColumnIndex = this._daoSaveColumnsMapping.CardNumberColumnIndex;
        columnsMapping.CardNumberColumnText = this._daoSaveColumnsMapping.CardNumberColumnText;
        columnsMapping.AmountColumnIndex = this._daoSaveColumnsMapping.AmountColumnIndex;
        columnsMapping.AmountColumnText = this._daoSaveColumnsMapping.AmountColumnText;
        columnsMapping.DateInscriptionColumnIndex = this._daoSaveColumnsMapping.DateInscriptionColumnIndex;
        columnsMapping.DateInscriptionColumnText = this._daoSaveColumnsMapping.DateInscriptionColumnText;
        columnsMapping.DescriptionColumnIndex = this._daoSaveColumnsMapping.DescriptionColumnIndex;
        columnsMapping.DescriptionColumnText = this._daoSaveColumnsMapping.DescriptionColumnText;

        this._dataContext.ColumnsMappings.Update(columnsMapping);
        await this._dataContext.SaveChangesAsync();

        this.SetSucceeded(id);
    }
}
