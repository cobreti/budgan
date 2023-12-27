using BudganEngine.Model;

namespace BudganEngineTest.Model;

public class CsvTransactionOutTest
{
    public BankTransaction Transaction { get; }
    
    public CsvTransactionOutTest()
    {
        Transaction = new()
        {
            DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            DateInscription = DateOnly.FromDateTime(DateTime.Now),
            CardNumber = "1234",
            Amount = "100.0",
            Description = "something",
            LayoutName = "test-layout",
            Source = new()
            {
                FileRelativePath = "path",
                InputId = "inputId"
            }
        };
    }

    [Fact]
    public void ConstructionWithoutDateFormat()
    {
        var csvOut = new CsvTransactionOut(Transaction, null);
        var origin = $"{Transaction.Source.InputId} : {Transaction.Source.FileRelativePath}";

        Assert.Equal(Transaction.CardNumber, csvOut.CardNumber);
        Assert.Equal(Transaction.DateTransaction.ToString(), csvOut.DateTransaction);
        Assert.Equal(Transaction.DateInscription.ToString(), csvOut.DateInscription);
        Assert.Equal(Transaction.Amount, csvOut.Amount);
        Assert.Equal(Transaction.Description, csvOut.Description);
        Assert.Equal(Transaction.LayoutName, csvOut.LayoutName);
        Assert.Equal(origin, csvOut.Origin);
    }
    
    [Fact]
    public void ConstructionWithDateFormat()
    {
        var dateFormat = "yyyyMMdd";
        var csvOut = new CsvTransactionOut(Transaction, dateFormat);
        var origin = $"{Transaction.Source.InputId} : {Transaction.Source.FileRelativePath}";

        Assert.Equal(Transaction.CardNumber, csvOut.CardNumber);
        Assert.Equal(Transaction.DateTransaction.ToString(dateFormat), csvOut.DateTransaction);
        Assert.Equal(Transaction.DateInscription.ToString(dateFormat), csvOut.DateInscription);
        Assert.Equal(Transaction.Amount, csvOut.Amount);
        Assert.Equal(Transaction.Description, csvOut.Description);
        Assert.Equal(Transaction.LayoutName, csvOut.LayoutName);
        Assert.Equal(origin, csvOut.Origin);
    }

}