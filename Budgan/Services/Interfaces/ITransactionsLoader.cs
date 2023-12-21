namespace Budgan.Services.Interfaces;

public interface ITransactionsLoader
{
    void Load(string key, string path, string layout);
}
