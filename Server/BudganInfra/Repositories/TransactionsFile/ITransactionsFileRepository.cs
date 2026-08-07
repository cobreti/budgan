using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BudganInfra.Repositories.TransactionsFile.Save;

namespace BudganInfra.Repositories.TransactionsFile
{
    public interface ITransactionsFileRepository
    {
        ISaveTransactionsFileRepoOp SaveTransactionsFileRepoOperation(DaoSaveTransactionsFile daoSaveTransactionsFile);
    }
}
