using Budgan.Model;

namespace Budgan.Core;

public interface ITransactionsContainer
{
    string LayoutName { get; }
    
    string Origin { get; }

    void Add(BankTransaction bankTransaction);

    IEnumerable<BankTransaction> GetAllTransactions();
}
