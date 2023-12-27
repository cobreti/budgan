using BudganEngine.Model;

namespace BudganEngineTest.Model;

public class BankTransactionsLayoutTest
{
    public BankTransactionsLayout Layout { get; }

    public int DateTransactionIndex { get; } = 1;
    public int DateInscriptionIndex { get; } = 2;
    public int AmountIndex { get; } = 3;
    public int DescriptionIndex { get; } = 4;
    public int CardNumberIndex { get; } = 5;
    public int MinColumnsRequired { get; } = 2; 

    public BankTransactionsLayoutTest()
    {
        Layout = new()
        {
            Name = "test",
            DateTransaction = DateTransactionIndex,
            DateInscription = DateInscriptionIndex,
            Amount = AmountIndex,
            Description = DescriptionIndex,
            CardNumber = CardNumberIndex,
            Key = new string[] { "DateTransaction", "Description", "CardNumber" },
            MinColumnsRequired = MinColumnsRequired
        };
    }

    [Fact]
    public void StaticConstruction()
    {
        Assert.Equal(5, BankTransactionsLayout.IndexByNameMapping.Count);

        var keys = BankTransactionsLayout.IndexByNameMapping.Keys;
        
        Assert.Contains("datetransaction", keys);
        Assert.Contains("dateinscription", keys);
        Assert.Contains("amount", keys);
        Assert.Contains("description", keys);
        Assert.Contains("cardnumber", keys);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(DateTransactionIndex, Layout.DateTransaction);
        Assert.Equal(DateInscriptionIndex, Layout.DateInscription);
        Assert.Equal(AmountIndex, Layout.Amount);
        Assert.Equal(DescriptionIndex, Layout.Description);
        Assert.Equal(CardNumberIndex, Layout.CardNumber);
        Assert.Equal(3, Layout.Key.Length);
        Assert.Equal("test", Layout.Name);
        Assert.Equal(MinColumnsRequired, Layout.MinColumnsRequired);
    }

    [Fact]
    public void GetDateTransactionByName()
    {
        var col = Layout.GetIndexByName("DateTransaction");

        Assert.Equal(DateTransactionIndex, col);
    }

    [Fact]
    public void GetDateTransactionIndexByName()
    {
        var col = Layout.GetIndexByName("DateTransactionIndex");

        Assert.Equal(DateTransactionIndex, col);
    }

    [Fact]
    public void GetDateInscriptionByName()
    {
        var col = Layout.GetIndexByName("DateInscription");

        Assert.Equal(DateInscriptionIndex, col);
    }

    [Fact]
    public void GetAmountByName()
    {
        var col = Layout.GetIndexByName("Amount");

        Assert.Equal(AmountIndex, col);
    }

    [Fact]
    public void GetDescriptionByName()
    {
        var col = Layout.GetIndexByName("Description");

        Assert.Equal(DescriptionIndex, col);
    }

    [Fact]
    public void GetCardNumberByName()
    {
        var col = Layout.GetIndexByName("CardNumber");

        Assert.Equal(CardNumberIndex, col);
    }

    [Fact]
    public void InvalidColumnNameThrows()
    {
        Assert.Throws<ArgumentException>(() => { Layout.GetIndexByName("invalid"); });
    }
}
