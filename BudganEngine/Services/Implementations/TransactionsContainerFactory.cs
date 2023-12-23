using BudganEngine.Core;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Services.Implementations;

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
