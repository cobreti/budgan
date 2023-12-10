using System.Globalization;
using System.IO.Abstractions;
using Ardalis.GuardClauses;
using Budgan.Model;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace Budgan.Services;

public class TransactionsWriter : ITransactionsWriter
{
    public ILogger<TransactionsWriter>  Logger { get; }
    
    public IState State { get; }
    
    public ITransactionsMgr TransactionsMgr { get; }
    
    public IFileSystem FileSystem { get; }

    public TransactionsWriter(
        ILogger<TransactionsWriter> logger,
        ITransactionsMgr transactionsMgr,
        IFileSystem fileSystem,
        IState state)
    {
        Logger = logger;
        State = state;
        TransactionsMgr = transactionsMgr;
        FileSystem = fileSystem;
    }

    public void Write(string filename, IEnumerable<Transaction> transactions)
    {
        Guard.Against.Null(State.Output);
        
        string outputDir = State.Output;

        if (!FileSystem.Directory.Exists(outputDir))
        {
            FileSystem.Directory.CreateDirectory(outputDir);
        }

        string outputFile = FileSystem.Path.Combine(outputDir, filename);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = "\r\n",
            HasHeaderRecord = true
        };
        
        using (var outputStream = FileSystem.File.CreateText(outputFile))
        using (var  csvWriter = new CsvWriter(outputStream, config))
        {
            outputStream.NewLine = "\r\n";
            
            csvWriter.WriteHeader<CsvTransactionOut>();
            csvWriter.Flush();
            csvWriter.NextRecord();
            
            foreach (var transaction in transactions)
            {
                var csvTransaction = new CsvTransactionOut(transaction);
                csvWriter.WriteRecord<CsvTransactionOut>(csvTransaction);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }
        }
    }
}


