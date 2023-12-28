using CommandLine;

namespace Budgan.Options.Runtime;

public class CommandLineOptions
{
    [Option(HelpText = "json configuration files")]
    public IEnumerable<string> Config { get; set; }
}
