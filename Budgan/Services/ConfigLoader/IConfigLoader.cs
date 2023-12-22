namespace Budgan.Services.ConfigLoader;

public interface IConfigLoader
{
    void AddFromFile(string file);

    void ProcessConfig();
}
