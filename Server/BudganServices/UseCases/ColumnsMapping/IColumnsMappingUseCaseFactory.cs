using BudganServices.UseCases.ColumnsMapping.AddOrUpdate;
using BudganServices.UseCases.ColumnsMapping.Delete;
using BudganServices.UseCases.ColumnsMapping.Get;
using BudganServices.UseCases.ColumnsMapping.GetList;

namespace BudganServices.UseCases.ColumnsMapping;

public interface IColumnsMappingUseCaseFactory
{
    IAddOrUpdateColumnsMappingUseCase AddOrUpdateUseCase(BOAddOrUpdateColumnsMapping model);
    IListColumnsMappingUseCase  ListColumnsMappingUseCase();
    IGetColumnsMappingUseCase GetColumnsMappingUseCase(Guid id);
    IDeleteColumnsMappingUseCase DeleteColumnsMappingUseCase(Guid id);
}