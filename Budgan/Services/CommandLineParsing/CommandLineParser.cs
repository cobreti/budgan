using Budgan.Core.ConfigLoader;
using Budgan.Options;
using Budgan.Options.Runtime;
using Budgan.Services.Interfaces;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Budgan.Services.CommandLineParsing;

public class CommandLineParser : ICommandLineParser
{
    public ILogger<CommandLineParser> Logger { get; }
    
    public IBankTransactionLayoutSettings LayoutSettings { get; }
    
    public ITransactionsLoader TransactionsLoader { get; }
    
    public IConfigLoaderFactory ConfigLoaderFactory { get; }

    public List<IConfigLoader> ConfigLoaders { get; } = new();

    public CommandLineParser(
        ILogger<CommandLineParser> logger,
        ITransactionsLoader transactionsLoader,
        IConfigLoaderFactory configLoaderFactory,
        IBankTransactionLayoutSettings layoutSettings )
    {
        Logger = logger;
        LayoutSettings = layoutSettings;
        TransactionsLoader = transactionsLoader;
        ConfigLoaderFactory = configLoaderFactory;
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
                
                ConfigLoaders.ForEach(loader => loader.ProcessLayout());
                ConfigLoaders.ForEach(loader => loader.ProcessInput());
                ConfigLoaders.ForEach(loader => loader.ProcessOutputs());
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
                    var loader = ConfigLoaderFactory.Create(config.TransactionLayouts);
                    ConfigLoaders.Add(loader);
                }

                if (config?.Inputs != null)
                {
                    var loader = ConfigLoaderFactory.Create(config.Inputs);
                    ConfigLoaders.Add(loader);
                }

                Logger.LogDebug(json);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }
}
