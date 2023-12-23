using BudganEngine.Model;

namespace BudganEngine.Services.Interfaces;

public interface ITransactionParser
{
    void Parse(string fileId, string file, StreamReader streamReader, BankTransactionsLayout layout);
}
