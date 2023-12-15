using Budgan.Core.ConfigLoader;
using Budgan.Options;

namespace Budgan.Services.CommandLineParsing;

public interface IConfigLoaderFactory
{
    IConfigLoader Create(TransactionLayoutsConfigMap transactionsLayout);
    IConfigLoader Create(InputsConfigMap inputsConfig);
    IConfigLoader Create(OutputsConfigMap outputsConfig);
}
