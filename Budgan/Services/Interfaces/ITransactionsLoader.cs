namespace Budgan.Services.Interfaces;

public interface ITransactionsLoader
{
    void Load(string path, string layout);
}
