using Budgan.Model;

namespace Budgan.Services.Interfaces;

public interface ITransactionsWriter
{
    void Write(string filepath, IEnumerable<BankTransaction> transactions);
}
