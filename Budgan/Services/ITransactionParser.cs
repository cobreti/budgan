using CsvHelper;

namespace Budgan.Services;

public interface ITransactionParser
{
    void Parse(string origin, IParser parser);
}