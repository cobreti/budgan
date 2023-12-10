using CsvHelper.Configuration.Attributes;

namespace Budgan.Model;

public class CsvTransactionOut
{
    [Index(0), Name("Card Number")] public string CardNumber => Transaction.CardNumber;

    [Index(1), Name("Date Transaction")] public string DateTransaction => Transaction.DateTransaction;

    [Index(2), Name("Date Inscription")] public string DateInscription => Transaction.DateInscription;

    [Index(3), Name("Amount")] public string Amount => Transaction.Amount;

    [Index(4), Name("Description")] public string Description => Transaction.Description;

    [Index(5), Name("LayoutName")] public string LayoutName => Transaction.LayoutName;
    
    [Index(6), Name("Origin")] public string Origin => Transaction.Origin;

    [Ignore]
    public Transaction Transaction { get; }

    public CsvTransactionOut(Transaction transaction)
    {
        Transaction = transaction;
    }
}