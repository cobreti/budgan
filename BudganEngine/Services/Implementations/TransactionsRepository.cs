using BudganEngine.Core;
using BudganEngine.Model;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Services.Implementations;

public class TransactionsRepository : ITransactionsRepository
{
    public Dictionary<string, ITransactionsContainer> Containers { get; } = new();
    
    public ILogger<TransactionsRepository>     Logger { get; }

    public ITransactionsContainerFactory TransactionsContainerFactory { get; }

    public TransactionsRepository(
        ILogger<TransactionsRepository> logger,
        ITransactionsContainerFactory transactionsContainerFactory)
    {
        Logger = logger;
        TransactionsContainerFactory = transactionsContainerFactory;
    }

    public void Add(BankTransaction bankTransaction)
    {
        var container = GetContainerForTransaction(bankTransaction);

        container.Add(bankTransaction);
    }
    
    public ITransactionsContainer GetContainerForTransaction(BankTransaction bankTransaction)
    {
        if (Containers.TryGetValue(bankTransaction.Source.InputId, out var containerForTransaction))
        {
            return containerForTransaction;
        }

        var container =
            TransactionsContainerFactory.CreateTransactionsContainer(bankTransaction.LayoutName, bankTransaction.Source.InputId);
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

