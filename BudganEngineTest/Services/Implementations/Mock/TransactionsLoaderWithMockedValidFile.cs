using System.IO.Abstractions;
using BudganEngine.Services.Implementations;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngineTest.Services.Implementations.Mock;

public class TransactionsLoaderWithMockedValidFile : TransactionsLoader
{
    public bool ValidFile { get; set; } = true;

    public TransactionsLoaderWithMockedValidFile(
        ILogger<TransactionsLoader> logger,
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
