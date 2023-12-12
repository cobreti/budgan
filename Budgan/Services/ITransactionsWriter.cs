using Budgan.Model;

namespace Budgan.Services;

public interface ITransactionsWriter
{
    void Write(string filename, IEnumerable<BankTransaction> transactions);
}
