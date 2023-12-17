using Budgan.Model;

namespace Budgan.Services.Interfaces;

public interface ITransactionsWriter
{
    void Write(string filePath, string? dateFormat, IEnumerable<BankTransaction> transactions);
}
