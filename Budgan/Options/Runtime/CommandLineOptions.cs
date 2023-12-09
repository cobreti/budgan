using CommandLine;

namespace Budgan.Options.Runtime;

public class CommandLineOptions
{
    [Option(Required = true, HelpText = "CSV Source path")]
    public string Source { get; set; }
    
    [Option(Required = true, HelpText = "output folder")]
    public string Output { get; set; }
    
    [Option(Required = true, HelpText = "CSV Layout to use")]
    public string Layout { get; set; }
}
