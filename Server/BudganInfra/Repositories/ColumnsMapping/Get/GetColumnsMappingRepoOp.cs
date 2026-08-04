using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.ColumnsMapping.Get;

public class GetColumnsMappingRepoOp : BaseRepositoryOperationWithResultValue<DaoGetColumnsMapping>, IGetColumnsMappingRepoOp
{
    private readonly DataContext _dataContext;
    private readonly Guid _id;

    public GetColumnsMappingRepoOp(DataContext dataContext, Guid id)
    {
        this._dataContext = dataContext;
        this._id = id;
    }

    public async Task ExecuteAsync()
    {
        var entity = await this._dataContext.ColumnsMappings
            .FirstOrDefaultAsync(x => x.Id == this._id);

        if (entity != null)
        {
            this.SetSucceeded(new DaoGetColumnsMapping
            {
                Id = entity.Id.ToString(),
                Name = entity.Name,
                CardNumberColumnIndex = entity.CardNumberColumnIndex,
                CardNumberColumnText = entity.CardNumberColumnText,
                DateInscriptionColumnIndex = entity.DateInscriptionColumnIndex,
                DateInscriptionColumnText = entity.DateInscriptionColumnText,
                AmountColumnIndex = entity.AmountColumnIndex,
                AmountColumnText = entity.AmountColumnText,
                DescriptionColumnIndex = entity.DescriptionColumnIndex,
                DescriptionColumnText = entity.DescriptionColumnText
            });
        }
        else
        {
            this.SetFailed();
        }
    }
}
