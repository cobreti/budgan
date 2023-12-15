using Budgan.Model;

namespace Budgan.Services.Interfaces;

public interface ITransactionParser
{
    void Parse(string file, StreamReader streamReader, BankTransactionsLayout layout);
}
