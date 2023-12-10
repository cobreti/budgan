using Budgan.Extensions;
using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Core;

public class TransactionsContainer : ITransactionsContainer
{
    public ILogger<TransactionsContainer>   Logger { get; }

    public Dictionary<string, Transaction> Transactions { get; } = new();

    public string LayoutName { get; }
    
    public string Origin { get; }
    
    public TransactionsContainer(
        string layoutName,
        string origin,
        ILogger<TransactionsContainer> logger)
    {
        Logger = logger;
        Origin = origin;
        LayoutName = layoutName;
    }

    public void Add(Transaction transaction)
    {
        if (Transactions.ContainsKey(transaction.Key))
        {
            Logger.LogTransaction("transaction already present in container", transaction);
            return;
        }
        
        Transactions.Add(transaction.Key, transaction);
    }

    public IEnumerable<Transaction> GetAllTransactions()
    {
        foreach (var transaction in Transactions.Values)
        {
            yield return transaction;
        }
    }
}

