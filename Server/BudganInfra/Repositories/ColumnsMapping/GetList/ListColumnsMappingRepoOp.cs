using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.ColumnsMapping.GetList;

internal class ListColumnsMappingRepoOp : BaseRepositoryOperationWithResultValue<List<DaoListColumnsMapping>>, IListColumnsMappingRepoOp
{
    private readonly DataContext _dataContext;
    private List<DaoListColumnsMapping> _daoGetListColumnsMappings = [];


    public ListColumnsMappingRepoOp(DataContext dataContext)
    {
        this._dataContext = dataContext;
    }

    public async Task ExecuteAsync()
    {
        var result = await this._dataContext.ColumnsMappings
            .Select(x => new DaoListColumnsMapping
            {
                Id = x.Id,
                Name = x.Name,
                CardNumberColumnIndex = x.CardNumberColumnIndex,
                CardNumberColumnText = x.CardNumberColumnText,
                DateInscriptionColumnIndex = x.DateInscriptionColumnIndex,
                DateInscriptionColumnText = x.DateInscriptionColumnText,
                AmountColumnIndex = x.AmountColumnIndex,
                AmountColumnText = x.AmountColumnText,
                DescriptionColumnIndex = x.DescriptionColumnIndex,
                DescriptionColumnText = x.DescriptionColumnText
            })
            .ToListAsync();
        
        this.SetSucceeded(result);
    }
}