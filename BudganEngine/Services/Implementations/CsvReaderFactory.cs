using BudganEngine.Services.Interfaces;
using CsvHelper.Configuration;

namespace BudganEngine.Services.Implementations;

public class CsvReaderFactory : ICsvReaderFactory
{
    public CsvHelper.IReader CreateReader(StreamReader streamReader, CsvConfiguration config)
    {
        return new CsvHelper.CsvReader(streamReader, config);
    }
}
