using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Options;

[ExcludeFromCodeCoverage]
public class AppConfig
{
    public string? DateFormat { get; set; } = "YYYYMMDD";
}
