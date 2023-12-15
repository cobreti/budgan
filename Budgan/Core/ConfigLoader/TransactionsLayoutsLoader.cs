using Budgan.Model;
using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Core.ConfigLoader;

public class TransactionsLayoutsLoader : IConfigLoader
{
    public TransactionLayoutsConfigMap TransactionLayoutsConfig { get; }
    public ILogger<TransactionsLayoutsLoader> Logger { get; }
    
    public IBankTransactionLayoutSettings LayoutSettings { get; }

    public TransactionsLayoutsLoader(
        TransactionLayoutsConfigMap transactionLayoutsConfig,
        ILogger<TransactionsLayoutsLoader> logger,
        IBankTransactionLayoutSettings layoutSettings)
    {
        TransactionLayoutsConfig = transactionLayoutsConfig;
        Logger = logger;
        LayoutSettings = layoutSettings;
    }
    
    public void ProcessLayout()
    {
        foreach (var (name, layout) in TransactionLayoutsConfig)
        {
            LayoutSettings.AddOrReplace(BankTransactionLayoutFromFileLayout(name, layout));
        }

    }

    public void ProcessInput()
    {
    }

    public void ProcessOutputs()
    {
    }
    
    public BankTransactionsLayout BankTransactionLayoutFromFileLayout(string name, FileLayout layout)
    {
        return new BankTransactionsLayout()
        {
            Name = name,
            MinColumnsRequired = layout.MinColumnsRequired,
            Amount = layout.Amount,
            CardNumber = layout.CardNumber,
            DateInscription = layout.DateInscription,
            DateTransaction = layout.DateTransaction,
            Description = layout.Description,
            Key = layout.Key
        };
    }

}
