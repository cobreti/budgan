using Budgan.Options;
using Microsoft.Extensions.Logging;

namespace Budgan.Core.ConfigLoader;

public class OutputsLoader : IConfigLoader
{
    public OutputsConfigMap Outputs { get; }
    public ILogger<OutputsLoader> Logger { get; }

    public OutputsLoader(
        OutputsConfigMap outputs,
        ILogger<OutputsLoader> logger)
    {
        Logger = logger;
        Outputs = outputs;
    }
    
    public void ProcessLayout()
    {
    }

    public void ProcessInput()
    {
    }

    public void ProcessOutputs()
    {
    }
}