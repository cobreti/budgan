using BudganInfra.DBContext;
using BudganInfra.Repositories.TransactionsFile.GetListByAccount;
using BudganInfra.Repositories.TransactionsFile.Save;

namespace BudganInfra.Repositories.TransactionsFile
{
    internal class TransactionsFileRepository : ITransactionsFileRepository
    {
        private readonly DataContext _dataContext;

        public TransactionsFileRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public ISaveTransactionsFileRepoOp SaveTransactionsFileRepoOperation(DaoSaveTransactionsFile daoSaveTransactionsFile)
        {
            return new SaveTransactionsFileRepoOp(this._dataContext, daoSaveTransactionsFile);
        }

        public IGetListByAccountTransactionsFileRepoOp GetListByAccountTransactionsFileRepoOperation(Guid accountId)
        {
            return new GetListByAccountTransactionsFileRepoOp(this._dataContext, accountId);
        }
    }
}