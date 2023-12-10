using Budgan.Model;

namespace Budgan.Core;

public interface ITransactionsContainer
{
    string LayoutName { get; }
    
    string Origin { get; }

    void Add(Transaction transaction);

    IEnumerable<Transaction> GetAllTransactions();
}
