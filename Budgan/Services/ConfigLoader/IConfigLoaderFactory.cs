using Budgan.Options;

namespace Budgan.Services.ConfigLoader;

public interface IConfigLoaderFactory
{
    TransactionsLayoutsSectionLoader Create(TransactionLayoutsConfigMap transactionsLayout);
    InputsSectionLoader Create(InputsConfigMap inputsConfig);
    OutputsSectionLoader Create(OutputsConfigMap outputsConfig);
}
