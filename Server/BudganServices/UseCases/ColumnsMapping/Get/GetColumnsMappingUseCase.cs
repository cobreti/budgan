using BudganInfra.Repositories.ColumnsMapping;

namespace BudganServices.UseCases.ColumnsMapping.Get;

public class GetColumnsMappingUseCase : BaseUseCaseWithResultValue<BOGetColumnsMapping>, IGetColumnsMappingUseCase
{
    private readonly IColumnsMappingRepository _repository;
    private readonly Guid _id;
    
    public GetColumnsMappingUseCase(IColumnsMappingRepository repository, Guid id)
    {
        this._repository = repository;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var repOp = this._repository.GetColumnsMappingRepoOperation(this._id);

        await repOp.ExecuteAsync();

        if (repOp.Succeeded)
        {
            var r = repOp.ResultValue;

            var result = new BOGetColumnsMapping
            {
                Id = r.Id,
                Name = r.Name,
                CardNumberColumnIndex = r.CardNumberColumnIndex,
                CardNumberColumnText = r.CardNumberColumnText,
                DateInscriptionColumnIndex = r.DateInscriptionColumnIndex,
                DateInscriptionColumnText = r.DateInscriptionColumnText,
                AmountColumnIndex = r.AmountColumnIndex,
                AmountColumnText = r.AmountColumnText,
                DescriptionColumnIndex = r.DescriptionColumnIndex,
                DescriptionColumnText = r.DescriptionColumnText,
            };
            
            this.SetSucceeded(result);
        }
        else
        {
            this.SetFailed();
        }
    }
}