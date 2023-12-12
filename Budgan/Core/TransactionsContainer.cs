using Budgan.Extensions;
using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Core;

public class TransactionsContainer : ITransactionsContainer
{
    public ILogger<TransactionsContainer>   Logger { get; }

    public Dictionary<string, BankTransaction> Transactions { get; } = new();

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

    public void Add(BankTransaction bankTransaction)
    {
        if (Transactions.ContainsKey(bankTransaction.Key))
        {
            Logger.LogTransaction("transaction already present in container", bankTransaction);
            return;
        }
        
        Transactions.Add(bankTransaction.Key, bankTransaction);
    }

    public IEnumerable<BankTransaction> GetAllTransactions()
    {
        foreach (var transaction in Transactions.Values)
        {
            yield return transaction;
        }
    }
}

