using Budgan.Options;
using Budgan.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Budgan.Services.ConfigLoader;

public class OutputsSectionLoader : IConfigSectionLoader
{
    public OutputsConfigMap Outputs { get; }
    public ILogger<OutputsSectionLoader> Logger { get; }
    
    public ITransactionsRepository TransactionsRepository { get; }
    
    public ITransactionsWriter TransactionsWriter { get; }

    public OutputsSectionLoader(
        OutputsConfigMap outputs,
        ITransactionsRepository transactionsRepository,
        ITransactionsWriter transactionsWriter,
        ILogger<OutputsSectionLoader> logger)
    {
        Logger = logger;
        Outputs = outputs;
        TransactionsRepository = transactionsRepository;
        TransactionsWriter = transactionsWriter;
    }
    
    public void Process()
    {
        foreach (var (name, outputConfig) in Outputs)
        {
            if (null != outputConfig.File)
            {
                var dateFormat = outputConfig.DateFormat?.Replace('Y', 'y').Replace('D', 'd');
                TransactionsWriter.Write(outputConfig.File, dateFormat, TransactionsRepository.GetAllTransactions());
            }
        }
    }
}
