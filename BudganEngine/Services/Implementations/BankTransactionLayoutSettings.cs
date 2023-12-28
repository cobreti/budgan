using BudganEngine.Model;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Services.Implementations;

public class BankTransactionLayoutSettings : IBankTransactionLayoutSettings
{
    public Dictionary<string, BankTransactionsLayout> Layouts { get; } = new();
    
    public ILogger<BankTransactionLayoutSettings> Logger { get; }

    public BankTransactionLayoutSettings(
        ILogger<BankTransactionLayoutSettings> logger)
    {
        Logger = logger;
    }

    public void AddOrReplace(BankTransactionsLayout layout)
    {
        Layouts[layout.Name] = layout;
    }

    public BankTransactionsLayout? GetByName(string layoutName)
    {
        return Layouts.GetValueOrDefault(layoutName);
    }
}
