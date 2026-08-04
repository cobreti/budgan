using BudganInfra.Repositories.ColumnsMapping;
using BudganInfra.Repositories.ColumnsMapping.Save;

namespace BudganServices.UseCases.ColumnsMapping.AddOrUpdate;

internal class AddOrUpdateColumnsMappingUseCase : BaseUseCaseWithResultValue<Guid>, IAddOrUpdateColumnsMappingUseCase
{
    private readonly IColumnsMappingRepository _columnsMappingRepository;
    private readonly BOAddOrUpdateColumnsMapping _boAddOrUpdateModel;

    public AddOrUpdateColumnsMappingUseCase(IColumnsMappingRepository columnsMappingRepository, BOAddOrUpdateColumnsMapping model)
    {
        this._columnsMappingRepository = columnsMappingRepository;
        this._boAddOrUpdateModel = model;
    }

    public async Task ExecuteAsync()
    {
        var daoSave = new DaoSaveColumnsMapping
        {
            Id = this._boAddOrUpdateModel.Id,
            Name = this._boAddOrUpdateModel.Name,
            CardNumberColumnIndex = this._boAddOrUpdateModel.CardNumberColumnIndex,
            CardNumberColumnText = this._boAddOrUpdateModel.CardNumberColumnText,
            DateInscriptionColumnIndex = this._boAddOrUpdateModel.DateInscriptionColumnIndex,
            DateInscriptionColumnText = this._boAddOrUpdateModel.DateInscriptionColumnText,
            AmountColumnIndex = this._boAddOrUpdateModel.AmountColumnIndex,
            AmountColumnText = this._boAddOrUpdateModel.AmountColumnText,
            DescriptionColumnIndex = this._boAddOrUpdateModel.DescriptionColumnIndex,
            DescriptionColumnText = this._boAddOrUpdateModel.DescriptionColumnText,
        };

        var repoOp = this._columnsMappingRepository.SaveColumnsMappingRepoOperation(daoSave);

        await repoOp.ExecuteAsync();

        this.SetSucceeded(repoOp.ResultValue);
    }
}
