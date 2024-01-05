using System.IO.Abstractions;
using Ardalis.GuardClauses;
using BudganEngine.Model;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Services.Implementations;

public class TransactionsLoader : ITransactionsLoader
{
    public ILogger<TransactionsLoader> Logger { get; }
    
    public IFileSystem FileSystem { get; }
    
    public ITransactionParser TransactionsParser { get; }
    
    public IBankTransactionLayoutSettings BankTransactionLayoutSettings { get; }
    
    public ICsvReaderFactory CsvReaderFactory { get; }
    
    public TransactionsLoader(
        ILogger<TransactionsLoader> logger,
        ITransactionParser transactionsParser,
        ICsvReaderFactory csvReaderFactory,
        IBankTransactionLayoutSettings bankTransactionLayoutSettings,
        IFileSystem fileSystem)
    {
        Logger = logger;
        FileSystem = fileSystem;
        TransactionsParser = transactionsParser;
        CsvReaderFactory = csvReaderFactory;
        BankTransactionLayoutSettings = bankTransactionLayoutSettings;
    }

    public void Load(string key, string path, string layoutName)
    {
        try
        {
            if (!FileSystem.Path.Exists(path))
            {
                Logger.LogError("source path {path} not found", path);
                throw new Exception($"source path {path} not found");
            }

            var layout = BankTransactionLayoutSettings.GetByName(layoutName);
            if (layout == null)
            {
                Logger.LogError("Invalid layout name {layoutName}", layoutName);
                throw new Exception($"Invalid layout name {layoutName}");
            }

            if (FileSystem.Directory.Exists(path))
            {
                var files = FileSystem.Directory.GetFiles(path);
                
                foreach (var file in files)
                {
                    var relPath = FileSystem.Path.GetRelativePath(path, file);
                    var fileId = $"{key} : {relPath}";
                    var transactionSource = new BankTransactionSource()
                    {
                        FileRelativePath = relPath,
                        InputId = key
                    };
                    Logger.LogDebug(relPath);
                    ReadFile(transactionSource, file, layout);
                }
            }

            if (FileSystem.File.Exists(path))
            {
                var transactionSource = new BankTransactionSource()
                {
                    FileRelativePath = path,
                    InputId = key
                };

                ReadFile(transactionSource, path, layout);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
    }

    public virtual bool IsValidFile(string file)
    {
        var extension = FileSystem.Path.GetExtension(file);
        return string.Compare(extension, ".csv", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public virtual void ReadFile(BankTransactionSource transactionSource, string file, BankTransactionsLayout layout)
    {
        if (!IsValidFile(file))
        {
            Logger.LogError("invalid file : {file}", file);
            throw new ArgumentException("invalid file");
        }
        
        Logger.LogDebug("source file : {0}", file);

        using var csvReader = CsvReaderFactory.CreateFromFile(file, layout.MinColumnsRequired ?? 1);
        Guard.Against.Null(csvReader, nameof(csvReader));
        // using var reader = TextReaderFactory.CreateFromFile(file);
        TransactionsParser.Parse(transactionSource, file, csvReader, layout);
    }
}
