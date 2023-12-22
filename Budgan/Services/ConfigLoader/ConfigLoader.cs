using Budgan.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Budgan.Services.ConfigLoader;

public class ConfigLoader : IConfigLoader
{
    public ILogger<ConfigLoader> Logger { get; }
    public IConfigLoaderFactory ConfigLoaderFactory { get; }

    public IConfigSectionRepository ConfigSectionRepository { get; }

    public ConfigLoader(
        ILogger<ConfigLoader> logger,
        IConfigSectionRepository configSectionRepository,
        IConfigLoaderFactory configLoaderFactory)
    {
        Logger = logger;
        ConfigSectionRepository = configSectionRepository;
        ConfigLoaderFactory = configLoaderFactory;
    }
    
    public void AddFromFile(string file)
    {
        try
        {
            using (var stream = new StreamReader(file))
            {
                var json = stream.ReadToEnd();
                var config = JsonConvert.DeserializeObject<Config>(json);

                if (config?.TransactionLayouts != null)
                {
                    var loader = ConfigLoaderFactory.Create(config.TransactionLayouts);
                    ConfigSectionRepository.Add(loader);
                }

                if (config?.Inputs != null)
                {
                    var loader = ConfigLoaderFactory.Create(config.Inputs);
                    ConfigSectionRepository.Add(loader);
                }

                if (config?.Outputs != null)
                {
                    var loader = ConfigLoaderFactory.Create(config.Outputs);
                    ConfigSectionRepository.Add(loader);
                }

                Logger.LogDebug(json);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
        }        
    }

    public void ProcessConfig()
    {
        foreach (var loader in ConfigSectionRepository.ListInProcessingOrder())
        {
            loader.Process();
        }
    }
}
