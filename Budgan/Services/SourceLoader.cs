using System.Data;
using System.Globalization;
using System.IO.Abstractions;
using System.Security.Claims;
// using Budgan.Model;
using Budgan.Options;
using Budgan.Options.Runtime;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Budgan.Services;

public class SourceLoader : ISourceLoader
{
    public ILogger<SourceLoader> Logger { get; }
    
    public IFileSystem FileSystem { get; }
    public IState State { get; }
    
    public SourceLoader(
        ILogger<SourceLoader> logger,
        IState state,
        IFileSystem fileSystem)
    {
        Logger = logger;
        FileSystem = fileSystem;
        State = state;
    }

    public void Load()
    {
        try
        {
            var sourcePath = State.Source;
            if (!FileSystem.Path.Exists(sourcePath))
            {
                throw new Exception($"source path {sourcePath} not found");
            }

            var files = FileSystem.Directory.GetFiles(sourcePath);
            
            foreach (var file in files)
            {
                if (IsValidFile(file))
                {
                    Logger.LogDebug("source file : {0}", file); // Process each file
                    ReadFile(file);
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e.Message);
            throw;
        }
    }

    public bool IsValidFile(string file)
    {
        var extension = FileSystem.Path.GetExtension(file);
        return string.Compare(extension, ".csv", StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    public void ReadFile(string file)
    {
        if (State.Layout == null)
            return;
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = (args) =>
            {
            },
            AllowComments = true,
            ShouldSkipRecord = (args) =>
            {
                if (args.Row.Parser.Count < State.MinColumnsRequired)
                {
                    return true;
                }
                
                return false;
            }
        };

        using (var reader = new StreamReader(file))
        using (var csv = new CsvReader(reader, config))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                string? dateTransaction = null;
                if (State.Layout.DateTransaction != null)
                {
                    dateTransaction = csv.Parser[State.Layout.DateTransaction.Value];
                }

                string? dateInscription = null;
                if (State.Layout.DateInscription != null)
                {
                    dateInscription = csv.Parser[State.Layout.DateInscription.Value];
                }

                string? amount = null;
                if (State.Layout.Amount != null)
                {
                    amount = csv.Parser[State.Layout.Amount.Value];
                }

                string? desc = null;
                if (State.Layout.Description != null)
                {
                    desc = csv.Parser[State.Layout.Description.Value];
                }

                // var rec = csv.GetRecord<CSVDataLine>();
                Logger.LogDebug("{date transaction}",dateTransaction);
                Logger.LogDebug("{date inscription}",dateInscription);
                Logger.LogDebug("{amount}",amount);
                Logger.LogDebug("{desc}",desc);
            }
        }
    }
}
