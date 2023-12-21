using Budgan.Model;

namespace Budgan.Services.Interfaces;

public interface ITransactionParser
{
    void Parse(string fileId, string file, StreamReader streamReader, BankTransactionsLayout layout);
}
