using System.Text;
using Ardalis.GuardClauses;
using Budgan.Model;
using Budgan.Options;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace Budgan.Services;

public class TransactionParser : ITransactionParser
{
    public ILogger<TransactionParser> Logger { get; }
    
    public IState State { get; }
    
    public ITransactionsMgr TransactionsMgr { get; }

    public TransactionParser(
        ILogger<TransactionParser> logger,
        ITransactionsMgr transactionsMgr,
        IState state)
    {
        Logger = logger;
        State = state;
        TransactionsMgr = transactionsMgr;
    }

    public void Parse(string origin, IParser parser)
    {
        var keyBuilder = new StringBuilder();

        if (Layout.Key != null)
        {
            foreach (var key in Layout.Key)
            {
                var index = Layout.GetIndexByName(key);
                if (index != null)
                {
                    keyBuilder.Append(GetParserColumn(parser, index));
                }
            }
        }

        var transaction = new Transaction()
        {
            Key = keyBuilder.ToString().Replace(" ", ""),
            LayoutName = LayoutName,
            Origin = origin,
            CardNumber = GetParserColumn(parser, Layout.CardNumber),
            DateTransaction = GetParserColumn(parser, Layout.DateTransaction),
            DateInscription = GetParserColumn(parser, Layout.DateTransaction),
            Amount = GetParserColumn(parser, Layout.Amount),
            Description = GetParserColumn(parser, Layout.Description)
        };

        TransactionsMgr.Add(transaction);
    }

    public string GetParserColumn(IParser parser, int? column)
    {
        if (column == null)
            return "";

        return parser[column.Value];
    }

    public string LayoutName
    {
        get
        {
            if (State.LayoutName == null)
                return "";

            return State.LayoutName;
        }
    }
    
    public FileLayout Layout
    {
        get
        {
            Guard.Against.Null(State.Layout);
            
            return State.Layout;
        }
    }
}
