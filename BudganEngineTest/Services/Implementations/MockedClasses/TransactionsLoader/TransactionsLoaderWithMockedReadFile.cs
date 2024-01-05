using System.IO.Abstractions;
using BudganEngine.Model;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngineTest.Services.Implementations.MockedClasses.TransactionsLoader;

public class TransactionsLoaderWithMockedReadFile : BudganEngine.Services.Implementations.TransactionsLoader
{
    public List<BankTransactionSource> TransactionSources { get; } = new();
    public List<string> Files { get; } = new();
    public List<BankTransactionsLayout> Layouts { get; } = new();
    
    public TransactionsLoaderWithMockedReadFile(
        ILogger<BudganEngine.Services.Implementations.TransactionsLoader> logger,
        ITransactionParser transactionsParser,
        ICsvReaderFactory csvReaderFactory,
        IBankTransactionLayoutSettings bankTransactionLayoutSettings,
        IFileSystem fileSystem) : base(logger, transactionsParser, csvReaderFactory, bankTransactionLayoutSettings, fileSystem)
    {
    }

    public override void ReadFile(BankTransactionSource transactionSource, string file, BankTransactionsLayout layout)
    {
        TransactionSources.Add(transactionSource);
        Files.Add(file);
        Layouts.Add(layout);
    }
}
