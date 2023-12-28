using System.Transactions;
using BudganEngine.Model;
using BudganEngine.Services.Indexes;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Services.Indexes;

public class TransactionsByDescriptionTest
{
    public Mock<ILogger<TransactionsByDescription>> LoggerMock { get; } = new();

    public Mock<ITransactionsRepository> TransactionsRepositoryMock { get; } = new();

    public TransactionsByDescription TransactionsByDescription { get; }
    
    public List<BankTransaction> Transactions { get; } = new()
    {
        new BankTransaction()
            {
                Key = "1",
                LayoutName = "Layout A",
                Source = new BankTransactionSource() { FileRelativePath = "Source A", InputId = "1"},
                Description = "Bill A",
                Amount = "100.00",
                CardNumber = "1234",
                DateInscription = DateOnly.FromDateTime(DateTime.Now),
                DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-2))
            },
        new BankTransaction()
            {
                Key = "2",
                LayoutName = "Layout B",
                Source = new BankTransactionSource() { FileRelativePath = "Source B", InputId = "2"},
                Description = "Bill B",
                Amount = "200.00",
                CardNumber = "12341",
                DateInscription = DateOnly.FromDateTime(DateTime.Now),
                DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-2))
            },
        new BankTransaction()
            {
                Key = "3",
                LayoutName = "Layout C",
                Source = new BankTransactionSource() { FileRelativePath = "Source C", InputId = "3"},
                Description = "Bill C",
                Amount = "240.00",
                CardNumber = "5478",
                DateInscription = DateOnly.FromDateTime(DateTime.Now),
                DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-2))
            },
        new BankTransaction()
            {
                Key = "4",
                LayoutName = "Layout D",
                Source = new BankTransactionSource() { FileRelativePath = "Source D", InputId = "4"},
                Description = "Bill B",
                Amount = "800.00",
                CardNumber = "12341",
                DateInscription = DateOnly.FromDateTime(DateTime.Now),
                DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-5))
            },
        new BankTransaction()
            {
                Key = "5",
                LayoutName = "Layout E",
                Source = new BankTransactionSource() { FileRelativePath = "Source E", InputId = "5"},
                Description = "Bill A",
                Amount = "9000.00",
                CardNumber = "1234",
                DateInscription = DateOnly.FromDateTime(DateTime.Now),
                DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
            }
    };
    
    public TransactionsByDescriptionTest()
    {
        TransactionsRepositoryMock.Setup(x => x.GetAllTransactions()).Returns(Transactions);
        TransactionsByDescription = new TransactionsByDescription(LoggerMock.Object, TransactionsRepositoryMock.Object);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, TransactionsByDescription.Logger);
        Assert.Equal(TransactionsRepositoryMock.Object, TransactionsByDescription.TransactionsRepository);
    }

    [Fact]
    public void Build()
    {
        TransactionsByDescription.Build();
        
        var keys = TransactionsByDescription.Index.Keys.ToList();
        
        Assert.Equal(3, keys.Count);
        Assert.Equal("Bill A", keys[0]);
        Assert.Equal("Bill B", keys[1]);
        Assert.Equal("Bill C", keys[2]);
        
        var billA = TransactionsByDescription.Index["Bill A"];
        Assert.Equal(2, billA.Count);
        Assert.Equal("1", billA[0].Key);
        Assert.Equal("5", billA[1].Key);
        
        var billB = TransactionsByDescription.Index["Bill B"];
        Assert.Equal(2, billB.Count);
        Assert.Equal("2", billB[0].Key);
        Assert.Equal("4", billB[1].Key);
        
        var billC = TransactionsByDescription.Index["Bill C"];
        Assert.Single(billC);
        Assert.Equal("3", billC[0].Key);
        
    }
}