using System.Globalization;
using System.Text;
using Ardalis.GuardClauses;
using Budgan.Model;
using Budgan.Options;
using Budgan.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.Implementations;

public class TransactionParser : ITransactionParser
{
    public ILogger<TransactionParser> Logger { get; }
    
    public ITransactionsRepository TransactionsRepository { get; }

    public TransactionParser(
        ILogger<TransactionParser> logger,
        ITransactionsRepository transactionsRepository)
    {
        Logger = logger;
        TransactionsRepository = transactionsRepository;
    }

    public void Parse(string file, StreamReader streamReader, BankTransactionsLayout layout)
    {
        var minColumnsRequired = layout.MinColumnsRequired ?? 2;
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = (args) =>
            {
            },
            AllowComments = true,
            ShouldSkipRecord = (args) => args.Row.Parser.Count < minColumnsRequired
        };
        
        using (var csv = new CsvReader(streamReader, config))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                ParseRow(file, csv.Parser, layout);
            }
        }
        
    }
    
    public void ParseRow(string origin, IParser parser, BankTransactionsLayout layout)
    {
        var keyBuilder = new StringBuilder();

        if (layout.Key != null)
        {
            foreach (var key in layout.Key)
            {
                var index = layout.GetIndexByName(key);
                if (index != null)
                {
                    keyBuilder.Append(GetParserColumn(parser, index));
                }
            }
        }

        var transaction = new BankTransaction()
        {
            Key = keyBuilder.ToString().Replace(" ", ""),
            LayoutName = layout.Name,
            Origin = origin,
            CardNumber = GetParserColumn(parser, layout.CardNumber),
            DateTransaction = GetParserColumn(parser, layout.DateTransaction),
            DateInscription = GetParserColumn(parser, layout.DateTransaction),
            Amount = GetParserColumn(parser, layout.Amount),
            Description = GetParserColumn(parser, layout.Description)
        };

        TransactionsRepository.Add(transaction);
    }

    public string GetParserColumn(IParser parser, int? column)
    {
        if (column == null)
            return "";

        return parser[column.Value];
    }
}
