using System.Globalization;
using System.IO.Abstractions;
using Ardalis.GuardClauses;
using Budgan.Model;
using Budgan.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.Implementations;

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

    public void Write(string filePath, IEnumerable<BankTransaction> transactions)
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
            var csvTransaction = new CsvTransactionOut(transaction);
            csvWriter.WriteRecord<CsvTransactionOut>(csvTransaction);
            csvWriter.NextRecord();
            csvWriter.Flush();
        }
    }
}


