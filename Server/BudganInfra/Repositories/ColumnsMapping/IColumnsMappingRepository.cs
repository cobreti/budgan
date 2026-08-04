using BudganInfra.Repositories.ColumnsMapping.Delete;
using BudganInfra.Repositories.ColumnsMapping.Get;
using BudganInfra.Repositories.ColumnsMapping.GetList;
using BudganInfra.Repositories.ColumnsMapping.Save;

namespace BudganInfra.Repositories.ColumnsMapping;

public interface IColumnsMappingRepository
{
    ISaveColumnsMappingRepoOp SaveColumnsMappingRepoOperation(DaoSaveColumnsMapping daoSaveColumnsMapping);
    IListColumnsMappingRepoOp ListColumnsMappingRepoOperation();
    IGetColumnsMappingRepoOp GetColumnsMappingRepoOperation(Guid id);
    IDeleteColumnsMappingRepoOp DeleteColumnsMappingRepoOperation(Guid id);
}
