using Budgan.Extensions;
using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Services;

public class TransactionsMgr : ITransactionMgr
{
    public ILogger<TransactionsMgr>     Logger { get; }


    public TransactionsMgr(
        ILogger<TransactionsMgr> logger)
    {
        Logger = logger;
    }

    public void Add(Transaction transaction)
    {
        Logger.LogTransaction("-->", transaction);
    }

    public void LogTransaction(string message, Transaction transaction)
    {
    }
}
