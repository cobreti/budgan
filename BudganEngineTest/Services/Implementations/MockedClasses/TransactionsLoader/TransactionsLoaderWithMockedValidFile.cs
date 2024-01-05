using System.IO.Abstractions;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngineTest.Services.Implementations.MockedClasses.TransactionsLoader;

public class TransactionsLoaderWithMockedValidFile : BudganEngine.Services.Implementations.TransactionsLoader
{
    public bool ValidFile { get; set; } = true;

    public TransactionsLoaderWithMockedValidFile(
        ILogger<BudganEngine.Services.Implementations.TransactionsLoader> logger,
        ITransactionParser transactionsParser,
        ICsvReaderFactory csvReaderFactory,
        IBankTransactionLayoutSettings bankTransactionLayoutSettings,
        IFileSystem fileSystem) : base(logger, transactionsParser, csvReaderFactory, bankTransactionLayoutSettings, fileSystem)
    {
    }

    public override bool IsValidFile(string file)
    {
        return ValidFile;
    }
}
