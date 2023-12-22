using Budgan.Core.ConfigLoader;
using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.CommandLineParsing;

public class ConfigLoaderFactory : IConfigLoaderFactory
{
    public ILoggerFactory LoggerFactory { get; }
    public ILogger<ConfigLoaderFactory> Logger { get; }
    public IBankTransactionLayoutSettings LayoutSettings { get; }
    public ITransactionsLoader TransactionsLoader { get; }

    public ITransactionsRepository TransactionsRepository { get; }
    public ITransactionsWriter TransactionsWriter { get; }
    
    public ConfigLoaderFactory(
        ILoggerFactory loggerFactory,
        ILogger<ConfigLoaderFactory> logger,
        ITransactionsLoader transactionsLoader,
        ITransactionsRepository transactionsRepository,
        ITransactionsWriter transactionsWriter,
        IBankTransactionLayoutSettings layoutSettings)
    {
        LoggerFactory = loggerFactory;
        Logger = logger;
        LayoutSettings = layoutSettings;
        TransactionsLoader = transactionsLoader;
        TransactionsRepository = transactionsRepository;
        TransactionsWriter = transactionsWriter;
    }
    
    public TransactionsLayoutsSectionLoader Create(TransactionLayoutsConfigMap transactionsLayout)
    {
        return new TransactionsLayoutsSectionLoader(
            transactionsLayout,
            LoggerFactory.CreateLogger<TransactionsLayoutsSectionLoader>(),
            LayoutSettings);
    }

    public InputsSectionLoader Create(InputsConfigMap inputsConfig)
    {
        return new InputsSectionLoader(
            inputsConfig,
            LoggerFactory.CreateLogger<InputsSectionLoader>(),
            TransactionsLoader);
    }

    public OutputsSectionLoader Create(OutputsConfigMap outputsConfig)
    {
        return new OutputsSectionLoader(
            outputsConfig,
            TransactionsRepository,
            TransactionsWriter,
            LoggerFactory.CreateLogger<OutputsSectionLoader>());
    }
    
}
