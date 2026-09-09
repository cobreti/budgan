using System.Globalization;
using BudganInfra.Repositories.AccountTransaction;

namespace BudganServices.UseCases.AccountTransaction.GetDateRange
{
    internal class GetDateRangeUseCase : BaseUseCaseWithResultValue<BOGetDateRange>, IGetDateRangeUseCase
    {
        private readonly IAccountTransactionRepository _accountTransactionRepository;
        private readonly Guid _accountId;

        public GetDateRangeUseCase(IAccountTransactionRepository repository, Guid accountId)
        {
            this._accountTransactionRepository = repository;
            this._accountId = accountId;
        }

        public async Task ExecuteAsync()
        {
            var repoOp = this._accountTransactionRepository.GetDateRangeRepoOp(this._accountId);

            await repoOp.ExecuteAsync();

            var result = new BOGetDateRange
            {
                StartDate = repoOp.ResultValue.StartDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                EndDate = repoOp.ResultValue.EndDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            };

            this.SetSucceeded(result);
        }
    }
}
