using Budgan.Model;

namespace Budgan.Services.Interfaces;

public interface IBankTransactionLayoutSettings
{
    void AddOrReplace(BankTransactionsLayout layout);

    BankTransactionsLayout? GetByName(string layoutName);
}