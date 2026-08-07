using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudganInfra.Repositories.TransactionsFile.Save
{
    public class DaoSaveTransactionsFile
    {
        public required Guid AccountId { get; set; }
        public required string Content { get; set; }
        public required string Filename { get; set; }
        public required DateOnly InsertionDate { get; set; }
    }
}
