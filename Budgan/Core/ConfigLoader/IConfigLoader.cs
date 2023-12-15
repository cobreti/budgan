namespace Budgan.Core.ConfigLoader;

public interface IConfigLoader
{
    void ProcessLayout();
    void ProcessInput();
    void ProcessOutputs();
}
