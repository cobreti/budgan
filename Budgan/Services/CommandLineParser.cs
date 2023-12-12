using Budgan.Options;
using Budgan.Options.Runtime;
using CommandLine;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Budgan.Services;

public class CommandLineParser : ICommandLineParser
{
    public ILogger<CommandLineParser> Logger { get; }

    public CommandLineParser(
        ILogger<CommandLineParser> logger)
    {
        Logger = logger;
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

                Logger.LogDebug(json);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }
    }
}
