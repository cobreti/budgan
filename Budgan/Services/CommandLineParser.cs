using Budgan.Model;
using Budgan.Options;
using Budgan.Options.Runtime;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Budgan.Services;

public class CommandLineParser : ICommandLineParser
{
    public ILogger<CommandLineParser> Logger { get; }
    
    public IBankTransactionLayoutSettings LayoutSettings { get; }

    public CommandLineParser(
        ILogger<CommandLineParser> logger,
        IBankTransactionLayoutSettings layoutSettings )
    {
        Logger = logger;
        LayoutSettings = layoutSettings;
    }

    public void Parse(string[] args)
    {
        var helpWriter = new StringWriter();
        var parser = new Parser(c =>
        {
            c.CaseSensitive = false;
            c.AutoVersion = false;
            c.AutoHelp = true;
            c.HelpWriter = helpWriter;
        });

        var helpText = helpWriter.ToString();
        if (helpText.Length > 0)
        {
            Console.WriteLine(helpText);
        }

        parser.ParseArguments<CommandLineOptions>(args)
            .WithParsed(c =>
            {
                foreach (var s in c.Config)
                {
                    ReadConfigFile(s);
                }
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    Logger.LogError("{error}", error);
                }
            });
    }

    public void ReadConfigFile(string file)
    {
        try
        {
            using (var stream = new StreamReader(file))
            {
                var json = stream.ReadToEnd();
                var config = JsonConvert.DeserializeObject<Config>(json);

                if (config?.TransactionLayouts != null)
                {
                    AddTransactionLayoutsToSettings(config.TransactionLayouts);
                }

                Logger.LogDebug(json);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }

    public void AddTransactionLayoutsToSettings(Dictionary<string, FileLayout> layouts)
    {
        foreach (var (name, layout) in layouts)
        {
            var transactionsLayout = new BankTransactionsLayout()
            {
                Name = name,
                Amount = layout.Amount,
                CardNumber = layout.CardNumber,
                DateInscription = layout.DateInscription,
                DateTransaction = layout.DateTransaction,
                Description = layout.Description,
                Key = layout.Key
            };
            LayoutSettings.AddOrReplace(transactionsLayout);
        }
    }
}
