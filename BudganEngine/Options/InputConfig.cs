using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Options;

[ExcludeFromCodeCoverage]
public class InputConfig
{
    public string? Source { get; set; }
    
    public string? Layout { get; set; }
}