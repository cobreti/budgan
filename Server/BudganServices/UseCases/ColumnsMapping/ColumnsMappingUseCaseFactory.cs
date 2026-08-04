using BudganInfra.Repositories.ColumnsMapping;
using BudganServices.UseCases.ColumnsMapping.AddOrUpdate;
using BudganServices.UseCases.ColumnsMapping.Delete;
using BudganServices.UseCases.ColumnsMapping.Get;
using BudganServices.UseCases.ColumnsMapping.GetList;

namespace BudganServices.UseCases.ColumnsMapping;

internal class ColumnsMappingUseCaseFactory : IColumnsMappingUseCaseFactory
{
    private readonly IColumnsMappingRepository _columnsMappingRepository;
    
    public ColumnsMappingUseCaseFactory(IColumnsMappingRepository columnsMappingRepository)
    {
        this._columnsMappingRepository = columnsMappingRepository;
    }

    public IAddOrUpdateColumnsMappingUseCase AddOrUpdateUseCase(BOAddOrUpdateColumnsMapping model)
    {
        return new AddOrUpdateColumnsMappingUseCase(this._columnsMappingRepository, model);
    }

    public IListColumnsMappingUseCase ListColumnsMappingUseCase()
    {
        return new ListColumnsMappingUseCase(this._columnsMappingRepository);
    }

    public IGetColumnsMappingUseCase GetColumnsMappingUseCase(Guid id)
    {
        return new GetColumnsMappingUseCase(this._columnsMappingRepository, id);
    }

    public IDeleteColumnsMappingUseCase DeleteColumnsMappingUseCase(Guid id)
    {
        return new DeleteColumnsMappingUseCase(this._columnsMappingRepository, id);
    }
}
