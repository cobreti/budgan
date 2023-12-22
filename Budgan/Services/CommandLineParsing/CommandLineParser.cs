using Budgan.Core.ConfigLoader;
using Budgan.Options.Runtime;
using CommandLine;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.CommandLineParsing;

public class CommandLineParser : ICommandLineParser
{
    public ILogger<CommandLineParser> Logger { get; }
    
    public IConfigLoader ConfigLoader { get; }

    public CommandLineParser(
        ILogger<CommandLineParser> logger,
        IConfigLoader configLoader)
    {
        Logger = logger;
        ConfigLoader = configLoader;
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
                    ConfigLoader.AddFromFile(s);
                }
                
                ConfigLoader.ProcessConfig();
            })
            .WithNotParsed(errors =>
            {
                foreach (var error in errors)
                {
                    Logger.LogError("{error}", error);
                }
            });
    }
}
