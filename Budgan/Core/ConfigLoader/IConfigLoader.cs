namespace Budgan.Core.ConfigLoader;

public interface IConfigLoader
{
    void AddFromFile(string file);

    void ProcessConfig();
}
