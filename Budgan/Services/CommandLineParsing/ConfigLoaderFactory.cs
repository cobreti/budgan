using Budgan.Core.ConfigLoader;
using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.CommandLineParsing;

public class ConfigLoaderFactory : IConfigLoaderFactory
{
    public ILoggerFactory LoggerFactory { get; }
    public ILogger<ConfigLoaderFactory> Logger { get; }
    public IBankTransactionLayoutSettings LayoutSettings { get; }
    public ITransactionsLoader TransactionsLoader { get; }

    public ConfigLoaderFactory(
        ILoggerFactory loggerFactory,
        ILogger<ConfigLoaderFactory> logger,
        ITransactionsLoader transactionsLoader,
        IBankTransactionLayoutSettings layoutSettings)
    {
        LoggerFactory = loggerFactory;
        Logger = logger;
        LayoutSettings = layoutSettings;
        TransactionsLoader = transactionsLoader;
    }
    
    public IConfigLoader Create(TransactionLayoutsConfigMap transactionsLayout)
    {
        return new TransactionsLayoutsLoader(
            transactionsLayout,
            LoggerFactory.CreateLogger<TransactionsLayoutsLoader>(),
            LayoutSettings);
    }

    public IConfigLoader Create(InputsConfigMap inputsConfig)
    {
        return new InputsLoader(
            inputsConfig,
            LoggerFactory.CreateLogger<InputsLoader>(),
            TransactionsLoader);
    }

    public IConfigLoader Create(OutputsConfigMap outputsConfig)
    {
        return new OutputsLoader(
            outputsConfig,
            LoggerFactory.CreateLogger<OutputsLoader>());
    }
    
}
