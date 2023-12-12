using Budgan.Model;
using Microsoft.Extensions.Logging;

namespace Budgan.Extensions;

public static class LoggerExtension
{
    public static void LogTransaction(this ILogger logger, string message, BankTransaction bankTransaction)
    {
        logger.LogDebug("{message} -- {layout name}\t{origin}\t{datetransaction}\t{dateinscription}\t{amount}\t{description}\t[{key}]",
            message,
            bankTransaction.LayoutName,
            bankTransaction.Origin,
            bankTransaction.DateTransaction,
            bankTransaction.DateInscription,
            bankTransaction.Amount,
            bankTransaction.Description,
            bankTransaction.Key);
    }
}