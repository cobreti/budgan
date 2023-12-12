namespace Budgan.Options;

public class Config
{
    public Dictionary<string, FileLayout>? TransactionLayouts { get; set; }
    
    public Dictionary<string, InputConfig>? Inputs { get; set; }
    
    public Dictionary<string, OutputConfig>? Outputs { get; set; }
}
