using CsvHelper.Configuration.Attributes;

namespace Budgan.Model;

public class CsvTransactionOut
{
    [Index(0), Name("Card Number")] public string CardNumber => BankTransaction.CardNumber;

    [Index(1), Name("Date Transaction")] public string DateTransaction => $"{BankTransaction.DateTransaction} - {BankTransaction.DateTransactionO.ToString()}";

    [Index(2), Name("Date Inscription")] public string DateInscription => BankTransaction.DateInscription;

    [Index(3), Name("Amount")] public string Amount => BankTransaction.Amount;

    [Index(4), Name("Description")] public string Description => BankTransaction.Description;

    [Index(5), Name("LayoutName")] public string LayoutName => BankTransaction.LayoutName;
    
    [Index(6), Name("Origin")] public string Origin => BankTransaction.Origin;

    [Ignore]
    public BankTransaction BankTransaction { get; }

    public CsvTransactionOut(BankTransaction bankTransaction)
    {
        BankTransaction = bankTransaction;
    }
}