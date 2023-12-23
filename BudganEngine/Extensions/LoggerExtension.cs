using BudganEngine.Model;
using Microsoft.Extensions.Logging;

namespace BudganEngine.Extensions;

public static class LoggerExtension
{
    public static void LogTransaction(this ILogger logger, string message, BankTransaction bankTransaction)
    {
        logger.LogDebug("{message} -- {layout name}\t{inputId}\t{relativePath}\t{datetransaction}\t{dateinscription}\t{amount}\t{description}\t[{key}]",
            message,
            bankTransaction.LayoutName,
            bankTransaction.Source.InputId,
            bankTransaction.Source.FileRelativePath,
            bankTransaction.DateTransaction,
            bankTransaction.DateInscription,
            bankTransaction.Amount,
            bankTransaction.Description,
            bankTransaction.Key);
    }
}