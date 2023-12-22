using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Core.ConfigLoader;

public class InputsSectionLoader : IConfigSectionLoader
{
    public InputsConfigMap Inputs { get; }
    public ILogger<InputsSectionLoader> Logger { get; }
    public ITransactionsLoader TransactionsLoader { get; }

    public InputsSectionLoader(
        InputsConfigMap inputs,
        ILogger<InputsSectionLoader> logger,
        ITransactionsLoader transactionsLoader )
    {
        Logger = logger;
        Inputs = inputs;
        TransactionsLoader = transactionsLoader;
    }
    
    public void Process()
    {
        foreach (var (name, inputConfig) in Inputs)
        {
            if (inputConfig.Source != null && inputConfig.Layout != null)
            {
                TransactionsLoader.Load(name, inputConfig.Source, inputConfig.Layout);
            }
            else
            {
                Logger.LogError("Invalid values for input {inputName}", name);
            }
        }
    }
}