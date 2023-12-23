using BudganEngine.Model;

namespace BudganEngine.Services.Interfaces;

public interface ITransactionParser
{
    void Parse(BankTransactionSource transactionSource, string file, StreamReader streamReader, BankTransactionsLayout layout);
}
