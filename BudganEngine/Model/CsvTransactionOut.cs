using CsvHelper.Configuration.Attributes;

namespace BudganEngine.Model;

public class CsvTransactionOut
{
    [Index(0), Name("Card Number")] public string CardNumber => BankTransaction.CardNumber;

    [Index(1), Name("Date Transaction")] public string DateTransaction => BankTransaction.DateTransaction.ToString(DateFormat);

    [Index(2), Name("Date Inscription")] public string DateInscription => BankTransaction.DateInscription.ToString(DateFormat);

    [Index(3), Name("Amount")] public string Amount => BankTransaction.Amount;

    [Index(4), Name("Description")] public string Description => BankTransaction.Description;

    [Index(5), Name("LayoutName")] public string LayoutName => BankTransaction.LayoutName;
    
    [Index(6), Name("Origin")] public string Origin => BankTransaction.Origin;

    [Ignore]
    public BankTransaction BankTransaction { get; }
    
    [Ignore]
    public string? DateFormat { get; }

    public CsvTransactionOut(BankTransaction bankTransaction, string? dateFormat)
    {
        BankTransaction = bankTransaction;
        DateFormat = dateFormat;
    }
}
