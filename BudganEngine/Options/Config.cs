using System.Diagnostics.CodeAnalysis;

namespace BudganEngine.Options;

[ExcludeFromCodeCoverage]
public class TransactionLayoutsConfigMap : Dictionary<string, FileLayout>
{
}

[ExcludeFromCodeCoverage]
public class InputsConfigMap : Dictionary<string, InputConfig>
{
}

[ExcludeFromCodeCoverage]
public class OutputsConfigMap : Dictionary<string, OutputConfig>
{
}

[ExcludeFromCodeCoverage]
public class Config
{
    public AppConfig? App { get; set; }
    
    public TransactionLayoutsConfigMap? TransactionLayouts { get; set; }
    
    public InputsConfigMap? Inputs { get; set; }
    
    public OutputsConfigMap? Outputs { get; set; }
}
