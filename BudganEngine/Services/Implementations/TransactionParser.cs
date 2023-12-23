using System.Globalization;
using System.Text;
using Ardalis.GuardClauses;
using BudganEngine.Model;
using BudganEngine.Options;
using BudganEngine.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BudganEngine.Services.Implementations;

public class TransactionParser : ITransactionParser
{
    public ILogger<TransactionParser> Logger { get; }
    
    public ITransactionsRepository TransactionsRepository { get; }
    
    public IOptions<AppConfig> AppConfigOptions { get; }
    
    public string DateFormat { get; }

    public TransactionParser(
        ILogger<TransactionParser> logger,
        IOptions<AppConfig> appConfigOptions,
        ITransactionsRepository transactionsRepository)
    {
        Logger = logger;
        AppConfigOptions = appConfigOptions;
        TransactionsRepository = transactionsRepository;
        
        Guard.Against.Null(AppConfigOptions.Value, message: "AppConfig not found : no date format available");
        Guard.Against.Null(AppConfigOptions.Value.DateFormat, message: "date format empty");

        DateFormat = AppConfigOptions.Value.DateFormat;
    }

    public void Parse(string fileId, string file, StreamReader streamReader, BankTransactionsLayout layout)
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
                ParseRow(fileId, csv.Parser, layout);
            }
        }
        
    }
    
    public void ParseRow(string origin, IParser parser, BankTransactionsLayout layout)
    {
        try
        {
            var keyBuilder = new StringBuilder();

            if (layout.Key != null)
            {
                foreach (var key in layout.Key)
                {
                    var index = layout.GetIndexByName(key);
                    if (index != null)
                    {
                        keyBuilder.Append(GetColumnValue(parser, index));
                    }
                }
            }
            
            var transaction = new BankTransaction()
            {
                Key = keyBuilder.ToString().Replace(" ", ""),
                LayoutName = layout.Name,
                Origin = origin,
                CardNumber = GetColumnValue(parser, layout.CardNumber),
                DateTransaction = GetDateColumnValue(parser, layout.DateTransaction),
                DateInscription = GetDateColumnValue(parser, layout.DateTransaction),
                Amount = GetColumnValue(parser, layout.Amount),
                Description = GetColumnValue(parser, layout.Description)
            };

            TransactionsRepository.Add(transaction);
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }
    
    public string GetColumnValue(IParser parser, int? column)
    {
        if (column == null)
            return "";

        return parser[column.Value];
    }

    public DateOnly GetDateColumnValue(IParser parser, int? column)
    {
        var value = GetColumnValue(parser, column);
        
        var res = DateOnly.TryParseExact(value, DateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateValue);
        if (!res)
        {
            throw new Exception($"Invalid date format for column: ${column}");
        }

        return dateValue;
    }
}
