using BudganEngine.Model;

namespace BudganEngine.Services.Interfaces;

public interface ITransactionsRepository
{
    void Add(BankTransaction bankTransaction);

    IEnumerable<BankTransaction> GetAllTransactions();
}