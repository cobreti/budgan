using CsvHelper.Configuration;

namespace BudganEngine.Services.Interfaces;

public interface ICsvReaderFactory
{
    CsvHelper.IReader CreateReader(StreamReader streamReader, CsvConfiguration config);
}

