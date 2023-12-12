using Budgan.Model;

namespace Budgan.Services;

public interface ITransactionsMgr
{
    void Add(BankTransaction bankTransaction);

    IEnumerable<BankTransaction> GetAllTransactions();
}