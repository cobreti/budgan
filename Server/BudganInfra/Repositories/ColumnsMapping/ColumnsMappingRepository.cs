using BudganInfra.DBContext;
using BudganInfra.Repositories.ColumnsMapping.Delete;
using BudganInfra.Repositories.ColumnsMapping.Get;
using BudganInfra.Repositories.ColumnsMapping.GetList;
using BudganInfra.Repositories.ColumnsMapping.Save;
using Microsoft.Extensions.Logging;

namespace BudganInfra.Repositories.ColumnsMapping;

internal class ColumnsMappingRepository : IColumnsMappingRepository
{
    private readonly DataContext dataContext;
    private readonly ILogger<ColumnsMappingRepository> logger;

    public ColumnsMappingRepository(DataContext dataContext, ILogger<ColumnsMappingRepository> logger)
    {
        this.dataContext = dataContext;
        this.logger = logger;
    }
    
    public ISaveColumnsMappingRepoOp SaveColumnsMappingRepoOperation(DaoSaveColumnsMapping daoSaveColumnsMapping)
    {
        return new SaveColumnsMappingRepoOp(this.dataContext, daoSaveColumnsMapping);
    }

    public IListColumnsMappingRepoOp ListColumnsMappingRepoOperation()
    {
        return new ListColumnsMappingRepoOp(this.dataContext);
    }

    public IGetColumnsMappingRepoOp GetColumnsMappingRepoOperation(Guid id)
    {
        return new GetColumnsMappingRepoOp(this.dataContext, id);
    }

    public IDeleteColumnsMappingRepoOp DeleteColumnsMappingRepoOperation(Guid id)
    {
        return new DeleteColumnsMappingRepoOp(this.dataContext, id);
    }
}
