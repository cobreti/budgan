using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Core.ConfigLoader;

public class OutputsLoader : IConfigLoader
{
    public OutputsConfigMap Outputs { get; }
    public ILogger<OutputsLoader> Logger { get; }
    
    public ITransactionsRepository TransactionsRepository { get; }
    
    public ITransactionsWriter TransactionsWriter { get; }

    public OutputsLoader(
        OutputsConfigMap outputs,
        ITransactionsRepository transactionsRepository,
        ITransactionsWriter transactionsWriter,
        ILogger<OutputsLoader> logger)
    {
        Logger = logger;
        Outputs = outputs;
        TransactionsRepository = transactionsRepository;
        TransactionsWriter = transactionsWriter;
    }
    
    public void ProcessLayout()
    {
    }

    public void ProcessInput()
    {
    }

    public void ProcessOutputs()
    {
        foreach (var (name, outputConfig) in Outputs)
        {
            if (null != outputConfig.File)
            {
                TransactionsWriter.Write(outputConfig.File, TransactionsRepository.GetAllTransactions());
            }
        }
    }
}
