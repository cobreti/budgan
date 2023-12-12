using Budgan.Core;
using Budgan.Extensions;
using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Services;

public class TransactionsesMgr : ITransactionsMgr
{
    public Dictionary<string, ITransactionsContainer> Containers { get; } = new();
    
    public ILogger<TransactionsesMgr>     Logger { get; }

    public ITransactionsContainerFactory TransactionsContainerFactory { get; }

    public TransactionsesMgr(
        ILogger<TransactionsesMgr> logger,
        ITransactionsContainerFactory transactionsContainerFactory)
    {
        Logger = logger;
        TransactionsContainerFactory = transactionsContainerFactory;
    }

    public void Add(BankTransaction bankTransaction)
    {
        var container = GetContainerForTransaction(bankTransaction);

        container.Add(bankTransaction);
        // Logger.LogTransaction("-->", transaction);
    }

    public void LogTransaction(string message, BankTransaction bankTransaction)
    {
    }

    public ITransactionsContainer GetContainerForTransaction(BankTransaction bankTransaction)
    {
        if (Containers.TryGetValue(bankTransaction.Origin, out var containerForTransaction))
        {
            return containerForTransaction;
        }

        var container =
            TransactionsContainerFactory.CreateTransactionsContainer(bankTransaction.LayoutName, bankTransaction.Origin);
        Containers.Add(container.Origin, container);

        return container;
    }

    public IEnumerable<BankTransaction> GetAllTransactions()
    {
        foreach (var containerEntry in Containers)
        {
            foreach (var transaction in containerEntry.Value.GetAllTransactions())
            {
                yield return transaction;
            }
        }
    }
}

