namespace Budgan.Core.ConfigLoader;

public interface IConfigSectionRepository
{
    void Add(InputsSectionLoader section);
    void Add(OutputsSectionLoader section);
    void Add(TransactionsLayoutsSectionLoader section);

    IEnumerable<IConfigSectionLoader> ListInProcessingOrder();
}
