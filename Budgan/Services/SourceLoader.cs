using Budgan.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Budgan.Services;

public class SourceLoader : ISourceLoader
{
    public ILogger<SourceLoader> Logger { get; }
    
    public IOptions<FoldersOptions> FolderOptions { get; }

    public SourceLoader(
        ILogger<SourceLoader> logger,
        IOptions<FoldersOptions> folderOptions)
    {
        Logger = logger;
        FolderOptions = folderOptions;
    }

    public void Load()
    {
        
    }
}
