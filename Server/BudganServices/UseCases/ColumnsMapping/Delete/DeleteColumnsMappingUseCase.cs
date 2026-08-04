using BudganInfra.Repositories.ColumnsMapping;

namespace BudganServices.UseCases.ColumnsMapping.Delete;

public class DeleteColumnsMappingUseCase : BaseUseCaseWithResultValue<Guid>, IDeleteColumnsMappingUseCase
{
    private readonly IColumnsMappingRepository _repository;
    private readonly Guid _id;
    
    public DeleteColumnsMappingUseCase(IColumnsMappingRepository repository, Guid id)
    {
        this._repository = repository;
        this._id = id;
    }
    
    public async Task ExecuteAsync()
    {
        var repoOp = this._repository.DeleteColumnsMappingRepoOperation(this._id);

        await repoOp.ExecuteAsync();

        if (repoOp.Succeeded)
        {
            this.SetSucceeded(this._id);
        }
        else
        {
            this.SetFailed();
        }
    }
}
