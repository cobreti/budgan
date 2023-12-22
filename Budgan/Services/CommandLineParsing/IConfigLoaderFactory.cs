using Budgan.Core.ConfigLoader;
using Budgan.Options;

namespace Budgan.Services.CommandLineParsing;

public interface IConfigLoaderFactory
{
    TransactionsLayoutsSectionLoader Create(TransactionLayoutsConfigMap transactionsLayout);
    InputsSectionLoader Create(InputsConfigMap inputsConfig);
    OutputsSectionLoader Create(OutputsConfigMap outputsConfig);
}
