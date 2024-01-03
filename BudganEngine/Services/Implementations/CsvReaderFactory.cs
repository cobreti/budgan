using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using BudganEngine.Services.Interfaces;
using CsvHelper.Configuration;

namespace BudganEngine.Services.Implementations;

[ExcludeFromCodeCoverage]
public class CsvReaderFactory : ICsvReaderFactory
{
    public CsvHelper.IReader CreateFromFile(string file, int minColumnsRequired)
    {
        var streamReader = new StreamReader(file);
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            AllowComments = true,
            ShouldSkipRecord = (args) => args.Row.Parser.Count < minColumnsRequired
        };

        return new CsvHelper.CsvReader(streamReader, config);
    }
}
