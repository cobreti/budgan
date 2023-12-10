using Budgan.Core;
using Microsoft.Extensions.Logging;

namespace Budgan.Services;

public class TransactionsContainerFactory : ITransactionsContainerFactory
{
    public ILogger<TransactionsContainerFactory> Logger { get; }
    
    public ILoggerFactory LoggerFactory { get; }

    public TransactionsContainerFactory(
        ILogger<TransactionsContainerFactory> logger,
        ILoggerFactory loggerFactory)
    {
        Logger = logger;
        LoggerFactory = loggerFactory;
    }

    public ITransactionsContainer CreateTransactionsContainer(string layoutName, string origin)
    {
        return new TransactionsContainer(
            layoutName,
            origin,
            LoggerFactory.CreateLogger<TransactionsContainer>());
    }
}
