using Budgan.Core;
using Budgan.Extensions;
using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Services;

public class TransactionsMgr : ITransactionMgr
{
    public Dictionary<string, ITransactionsContainer> Containers { get; } = new();
    
    public ILogger<TransactionsMgr>     Logger { get; }

    public ITransactionsContainerFactory TransactionsContainerFactory { get; }

    public TransactionsMgr(
        ILogger<TransactionsMgr> logger,
        ITransactionsContainerFactory transactionsContainerFactory)
    {
        Logger = logger;
        TransactionsContainerFactory = transactionsContainerFactory;
    }

    public void Add(Transaction transaction)
    {
        var container = GetContainerForTransaction(transaction);

        container.Add(transaction);
        // Logger.LogTransaction("-->", transaction);
    }

    public void LogTransaction(string message, Transaction transaction)
    {
    }

    public ITransactionsContainer GetContainerForTransaction(Transaction transaction)
    {
        if (Containers.TryGetValue(transaction.Origin, out var containerForTransaction))
        {
            return containerForTransaction;
        }

        var container =
            TransactionsContainerFactory.CreateTransactionsContainer(transaction.LayoutName, transaction.Origin);
        Containers.Add(container.Origin, container);

        return container;
    }
}
