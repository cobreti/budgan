using BudganInfra.DBContext;
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
    }
}