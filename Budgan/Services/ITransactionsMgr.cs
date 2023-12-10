using Budgan.Model;

namespace Budgan.Services;

public interface ITransactionsMgr
{
    void Add(Transaction transaction);

    IEnumerable<Transaction> GetAllTransactions();
}