using Budgan.Model;
using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.ConfigLoader;

public class TransactionsLayoutsSectionLoader : IConfigSectionLoader
{
    public TransactionLayoutsConfigMap TransactionLayoutsConfig { get; }
    public ILogger<TransactionsLayoutsSectionLoader> Logger { get; }
    
    public IBankTransactionLayoutSettings LayoutSettings { get; }

    public TransactionsLayoutsSectionLoader(
        TransactionLayoutsConfigMap transactionLayoutsConfig,
        ILogger<TransactionsLayoutsSectionLoader> logger,
        IBankTransactionLayoutSettings layoutSettings)
    {
        TransactionLayoutsConfig = transactionLayoutsConfig;
        Logger = logger;
        LayoutSettings = layoutSettings;
    }
    
    public void Process()
    {
        foreach (var (name, layout) in TransactionLayoutsConfig)
        {
            LayoutSettings.AddOrReplace(BankTransactionLayoutFromFileLayout(name, layout));
        }

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
