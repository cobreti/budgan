using CommandLine;

namespace Budgan.Options.Runtime;

public class CommandLineOptions
{
    // [Option(HelpText = "CSV Source path")]
    // public string Source { get; set; }
    //
    // [Option(HelpText = "output folder")]
    // public string Output { get; set; }
    //
    // [Option(HelpText = "CSV Layout to use")]
    // public string Layout { get; set; }

    [Option(HelpText = "json configuration files")]
    public IEnumerable<string> Config { get; set; }
}
