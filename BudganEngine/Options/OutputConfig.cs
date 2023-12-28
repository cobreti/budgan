using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Options;

[ExcludeFromCodeCoverage]
public class OutputConfig
{
    public string? File { get; set; }
    public string? DateFormat { get; set; }
}
