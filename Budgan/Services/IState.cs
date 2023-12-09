using Budgan.Options;

namespace Budgan.Services;

public interface IState
{
    bool Valid { get; }
    
    int MinColumnsRequired { get; }
    
    string? Source { get; }
    
    string? Output { get; }
    
    FileLayout? Layout { get; }

    void UpdateFromCommandLineArgs(string[] args);
}