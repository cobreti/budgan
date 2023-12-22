namespace Budgan.Core.ConfigLoader;

public class ConfigSectionRepository : IConfigSectionRepository
{
    public List<InputsSectionLoader> InputsSectionLoaders { get; } = new();

    public List<OutputsSectionLoader> OutputsSectionLoaders { get; } = new();

    public List<TransactionsLayoutsSectionLoader> TransactionsLayoutsSectionLoaders { get; } = new();

    
    public void Add(InputsSectionLoader section)
    {
        InputsSectionLoaders.Add(section);
    }

    public void Add(OutputsSectionLoader section)
    {
        OutputsSectionLoaders.Add(section);
    }

    public void Add(TransactionsLayoutsSectionLoader section)
    {
        TransactionsLayoutsSectionLoaders.Add(section);
    }

    public IEnumerable<IConfigSectionLoader> ListInProcessingOrder()
    {
        foreach (var loader in TransactionsLayoutsSectionLoaders)
        {
            yield return loader;
        }
        
        foreach (var loader in InputsSectionLoaders)
        {
            yield return loader;
        }

        foreach (var loader in OutputsSectionLoaders)
        {
            yield return loader;
        }
    }
}
