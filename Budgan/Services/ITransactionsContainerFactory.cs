using Budgan.Core;

namespace Budgan.Services;

public interface ITransactionsContainerFactory
{
    ITransactionsContainer CreateTransactionsContainer(string layoutName, string origin);
}