using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Core.ConfigLoader;

public class InputsLoader : IConfigLoader
{
    public InputsConfigMap Inputs { get; }
    public ILogger<InputsLoader> Logger { get; }
    public ITransactionsLoader TransactionsLoader { get; }

    public InputsLoader(
        InputsConfigMap inputs,
        ILogger<InputsLoader> logger,
        ITransactionsLoader transactionsLoader )
    {
        Logger = logger;
        Inputs = inputs;
        TransactionsLoader = transactionsLoader;
    }
    
    public void ProcessLayout()
    {
    }

    public void ProcessInput()
    {
        foreach (var (name, inputConfig) in Inputs)
        {
            if (inputConfig.Source != null && inputConfig.Layout != null)
            {
                TransactionsLoader.Load(inputConfig.Source, inputConfig.Layout);
            }
            else
            {
                Logger.LogError("Invalid values for input {inputName}", name);
            }
        }
    }

    public void ProcessOutputs()
    {
    }
}