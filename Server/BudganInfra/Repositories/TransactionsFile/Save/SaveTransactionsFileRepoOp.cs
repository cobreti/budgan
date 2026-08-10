using BudganGlobal.Errors;
using BudganInfra.DBContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace BudganInfra.Repositories.TransactionsFile.Save
{
    internal class SaveTransactionsFileRepoOp : BaseRepositoryOperationWithResultValue<Guid>, ISaveTransactionsFileRepoOp
    {
        private static readonly int[] UniqueConstraintViolationErrorNumbers = { 2601, 2627 };

        private readonly DataContext _dataContext;
        private readonly DaoSaveTransactionsFile _daoSaveTransactionsFile;

        public SaveTransactionsFileRepoOp(DataContext dataContext, DaoSaveTransactionsFile daoSaveTransactionsFile)
        {
            this._dataContext = dataContext;
            this._daoSaveTransactionsFile = daoSaveTransactionsFile;
        }

        public async Task ExecuteAsync()
        {
            var transactionsFileEntity = new DBContext.Entities.TransactionsFile
            {
                Id = Guid.CreateVersion7(),
                AccountId = this._daoSaveTransactionsFile.AccountId,
                Content = this._daoSaveTransactionsFile.Content,
                Filename = this._daoSaveTransactionsFile.Filename,
                InsertionDate = this._daoSaveTransactionsFile.InsertionDate
            };

            await _dataContext.AddAsync(transactionsFileEntity);

            if (!await this.TrySaveChanges())
            {
                return;
            }

            this.SetSucceeded(transactionsFileEntity.Id);
        }

        private async Task<bool> TrySaveChanges()
        {
            try
            {
                await this._dataContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
                this.SetFailed(BudganErrorValue.DuplicateTransactionsFile);
                return false;
            }
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sqlException
                   && sqlException.Errors.Cast<SqlError>().Any(e => UniqueConstraintViolationErrorNumbers.Contains(e.Number));
        }
    }
}
