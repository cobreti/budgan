using BudganInfra.DBContext;

namespace BudganInfra.Repositories.TransactionsFile.Save
{
    public class SaveTransactionsFileRepoOp : BaseRepositoryOperation, ISaveTransactionsFileRepoOp
    {
        private readonly DataContext _dataContext;
        private readonly DaoSaveTransactionsFile _daoSaveTransactionsFile;

        public SaveTransactionsFileRepoOp(DataContext dataContext, DaoSaveTransactionsFile daoSaveTransactionsFile)
        {
            this._dataContext = dataContext;
            this._daoSaveTransactionsFile = daoSaveTransactionsFile;
        }

        public async Task ExecuteAsync()
        {
            var transactionsFileEntity = new DBContext.Tables.TransactionsFile
            {
                Id = Guid.CreateVersion7(),
                AccountId = this._daoSaveTransactionsFile.AccountId,
                Content = this._daoSaveTransactionsFile.Content,
                Filename = this._daoSaveTransactionsFile.Filename,
                InsertionDate = this._daoSaveTransactionsFile.InsertionDate
            };

            await _dataContext.AddAsync(transactionsFileEntity);
            await _dataContext.SaveChangesAsync();
        }
    }
}
