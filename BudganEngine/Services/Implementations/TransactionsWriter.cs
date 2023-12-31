using System.Globalization;
using System.IO.Abstractions;
using BudganEngine.Model;
using BudganEngine.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Services.Implementations;

public class TransactionsWriter : ITransactionsWriter
{
    public ILogger<TransactionsWriter>  Logger { get; }
    
    public ITransactionsRepository TransactionsRepository { get; }
    
    public IFileSystem FileSystem { get; }

    public TransactionsWriter(
        ILogger<TransactionsWriter> logger,
        ITransactionsRepository transactionsRepository,
        IFileSystem fileSystem)
    {
        Logger = logger;
        TransactionsRepository = transactionsRepository;
        FileSystem = fileSystem;
    }

    public void Write(string filePath, string? dateFormat, IEnumerable<BankTransaction> transactions)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = "\r\n",
            HasHeaderRecord = true
        };

        using var outputStream = FileSystem.File.CreateText(filePath);
        using var  csvWriter = new CsvWriter(outputStream, config);
        
        outputStream.NewLine = "\r\n";
            
        csvWriter.WriteHeader<CsvTransactionOut>();
        csvWriter.Flush();
        csvWriter.NextRecord();
            
        foreach (var transaction in transactions)
        {
            var csvTransaction = new CsvTransactionOut(transaction, dateFormat);
            csvWriter.WriteRecord<CsvTransactionOut>(csvTransaction);
            csvWriter.NextRecord();
            csvWriter.Flush();
        }
    }
}


