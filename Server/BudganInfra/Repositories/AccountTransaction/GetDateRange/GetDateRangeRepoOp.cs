using BudganInfra.DBContext;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.AccountTransaction.GetDateRange
{
    internal class GetDateRangeRepoOp : BaseRepositoryOperationWithResultValue<DaoGetDateRange>, IGetDateRangeRepoOp
    {
        private readonly DataContext _dataContext;
        private readonly Guid _accountId;

        public GetDateRangeRepoOp(DataContext dataContext, Guid accountId)
        {
            this._dataContext = dataContext;
            this._accountId = accountId;
        }

        public async Task ExecuteAsync()
        {
            var result = new DaoGetDateRange
            {
                StartDate = null,
                EndDate = null
            };

            var hasData = await this._dataContext.AccountTransactions
                .Where(x => x.AccountId == this._accountId)
                .AnyAsync();

            if (hasData)
            {
                DateOnly? minDate = await this._dataContext.AccountTransactions
                    .Where(x => x.AccountId == this._accountId)
                    .MinAsync(x => x.DateInscription);
                DateOnly? maxDate = await this._dataContext.AccountTransactions
                    .Where(x => x.AccountId == this._accountId)
                    .MaxAsync(x => x.DateInscription);

                result.StartDate = minDate;
                result.EndDate = maxDate;
            }

            this.SetSucceeded(result);
        }
    }
}