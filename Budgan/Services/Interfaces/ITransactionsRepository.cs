using Budgan.Model;

namespace Budgan.Services.Interfaces;

public interface ITransactionsRepository
{
    void Add(BankTransaction bankTransaction);

    IEnumerable<BankTransaction> GetAllTransactions();
}