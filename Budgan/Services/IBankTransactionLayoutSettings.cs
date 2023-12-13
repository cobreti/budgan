using Budgan.Model;

namespace Budgan.Services;

public interface IBankTransactionLayoutSettings
{
    void AddOrReplace(BankTransactionsLayout layout);
}