namespace Budgan.Options;

public class TransactionLayoutsConfigMap : Dictionary<string, FileLayout>
{
}

public class InputsConfigMap : Dictionary<string, InputConfig>
{
}

public class OutputsConfigMap : Dictionary<string, OutputConfig>
{
    
}

public class Config
{
    public AppConfig? App { get; set; }
    
    public TransactionLayoutsConfigMap? TransactionLayouts { get; set; }
    
    public InputsConfigMap? Inputs { get; set; }
    
    public OutputsConfigMap? Outputs { get; set; }
}
