using System.IO.Abstractions;
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
    
    public TransactionsLoader(
        ILogger<TransactionsLoader> logger,
        ITransactionParser transactionsParser,
        IBankTransactionLayoutSettings bankTransactionLayoutSettings,
        IFileSystem fileSystem)
    {
        Logger = logger;
        FileSystem = fileSystem;
        TransactionsParser = transactionsParser;
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
                    Logger.LogDebug(relPath);
                    ReadFile(fileId, file, layout);
                }
            }

            if (FileSystem.File.Exists(path))
            {
                ReadFile(key, path, layout);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
    }

    public bool IsValidFile(string file)
    {
        var extension = FileSystem.Path.GetExtension(file);
        return string.Compare(extension, ".csv", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public void ReadFile(string fileId, string file, BankTransactionsLayout layout)
    {
        if (!IsValidFile(file))
        {
            Logger.LogError("invalid file found : {file}", file);
            return;
        }
        
        Logger.LogDebug("source file : {0}", file);

        using var reader = new StreamReader(file);
        TransactionsParser.Parse(fileId, file, reader, layout);
    }
}
