using BudganEngine.Model;

namespace BudganEngine.Services.Interfaces;

public interface ITransactionsWriter
{
    void Write(string filePath, string? dateFormat, IEnumerable<BankTransaction> transactions);
}
