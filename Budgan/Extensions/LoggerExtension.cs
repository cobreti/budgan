using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Extensions;

public static class LoggerExtension
{
    public static void LogTransaction(this ILogger logger, string message, Transaction transaction)
    {
        logger.LogDebug("{message} -- {layout name}\t{origin}\t{datetransaction}\t{dateinscription}\t{amount}\t{description}\t[{key}]",
            message,
            transaction.Layout,
            transaction.Origin,
            transaction.DateTransaction,
            transaction.DateInscription,
            transaction.Amount,
            transaction.Description,
            transaction.Key);
    }
}