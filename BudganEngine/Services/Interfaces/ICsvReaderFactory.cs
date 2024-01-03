using CsvHelper.Configuration;

namespace BudganEngine.Services.Interfaces;

public interface ICsvReaderFactory
{
    CsvHelper.IReader CreateFromFile(string file, int minColumnsRequired);
}

