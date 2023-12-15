using Budgan.Core;

namespace Budgan.Services.Interfaces;

public interface ITransactionsContainerFactory
{
    ITransactionsContainer CreateTransactionsContainer(string layoutName, string origin);
}