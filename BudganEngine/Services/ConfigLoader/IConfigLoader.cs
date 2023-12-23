namespace BudganEngine.Services.ConfigLoader;

public interface IConfigLoader
{
    void AddFromFile(string file);

    void ProcessConfig();
}
