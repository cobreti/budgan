using Budgan.Options;
using Budgan.Options.Runtime;
using CommandLine;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Budgan.Services;

public class State : IState
{
    public string? Source { get; set; }
    
    public string? Output { get; set; }

    public int MinColumnsRequired { get; set; }

    public string? LayoutName { get; set; }
    public FileLayout? Layout { get; set; }

    public bool Valid { get; set; } = false;
    
    public ILogger<State> Logger { get; }
    
    public IOptions<CsvSettings> CsvSettings { get; }

    public State(
        ILogger<State> logger,
        IOptions<CsvSettings> csvSettings)
    {
        Logger = logger;
        CsvSettings = csvSettings;

        MinColumnsRequired = CsvSettings.Value.MinColumnsRequired ?? 2;
    }

    public void UpdateFromCommandLineArgs(string[] args)
    {
        // var helpWriter = new StringWriter();
        // var parser = new Parser(c =>
        // {
        //     c.CaseSensitive = false;
        //     c.AutoVersion = false;
        //     c.AutoHelp = true;
        //     c.HelpWriter = helpWriter;
        // });
        //
        // var helpText = helpWriter.ToString();
        // if (helpText.Length > 0)
        // {
        //     Console.WriteLine(helpText);
        // }
        //
        // parser.ParseArguments<CommandLineOptions>(args)
        //     .WithParsed(c =>
        //     {
        //         // Source = c.Source;
        //         // Output = c.Output;
        //
        //         var entry = CsvSettings.Value.Layouts?.FirstOrDefault(x => x.Key == c.Layout);
        //         this.Layout = entry?.Value;
        //         this.LayoutName = entry?.Key;
        //     })
        //     .WithNotParsed(errors =>
        //     {
        //         foreach (var error in errors)
        //         {
        //             Logger.LogError("{error}", error);
        //         }
        //     });
        //
        // Validate();
    }

    public void Validate()
    {
        // Valid =
        // (
        //     Layout != null &&
        //     Source != null &&
        //     Output != null
        // );
    }
}
