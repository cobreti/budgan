using CommandLine;

namespace Budgan;

public class CommandLineOptions
{
    [Option(Required = true, HelpText = "CSV Source path")]
    public string Source { get; set; }
    
    [Option(Required = true, HelpText = "output folder")]
    public string Output { get; set; }
}
