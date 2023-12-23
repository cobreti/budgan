using BudganEngine.Model;

namespace BudganEngine.Services.Interfaces;

public interface IBankTransactionLayoutSettings
{
    void AddOrReplace(BankTransactionsLayout layout);

    BankTransactionsLayout? GetByName(string layoutName);
}