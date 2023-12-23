using BudganEngine.Core;

namespace BudganEngine.Services.Interfaces;

public interface ITransactionsContainerFactory
{
    ITransactionsContainer CreateTransactionsContainer(string layoutName, string origin);
}