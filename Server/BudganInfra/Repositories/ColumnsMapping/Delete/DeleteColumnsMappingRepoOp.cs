using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.ColumnsMapping.Delete;

public class DeleteColumnsMappingRepoOp : BaseRepositoryOperationWithResultValue<Guid>, IDeleteColumnsMappingRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _id;

    public DeleteColumnsMappingRepoOp(DataContext dataContext, Guid id)
    {
        this._dataContext = dataContext;
        this._id = id;
    }
    
    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.ColumnsMappings
            .FirstOrDefaultAsync(c => c.Id == this._id);

        if (entity == null)
        {
            this.SetFailed();
        }
        else
        {
            this._dataContext.ColumnsMappings.Remove(entity);
            await this._dataContext.SaveChangesAsync();

            this.SetSucceeded(this._id);
        }
    }
}
