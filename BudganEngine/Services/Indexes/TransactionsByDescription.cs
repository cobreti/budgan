using BudganEngine.Model;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Services.Indexes;

public class TransactionsByDescription : ITransactionsByDescription
{
    public ILogger<TransactionsByDescription> Logger { get; }
    
    public ITransactionsRepository TransactionsRepository { get; }

    public Dictionary<string, List<BankTransaction>> Index { get; } = new();

    public TransactionsByDescription(
        ILogger<TransactionsByDescription> logger,
        ITransactionsRepository transactionsRepository)
    {
        Logger = logger;
        TransactionsRepository = transactionsRepository;
    }

    public void Build()
    {
        foreach (var transaction in TransactionsRepository.GetAllTransactions())
        {
            if (Index.TryGetValue(transaction.Description, out var transList))
            {
                transList.Add(transaction);
            }
            else
            {
                Index.Add(transaction.Description, new List<BankTransaction>() { {transaction} });
            }
        }
    }
}
