using BudganEngine.Core;
using BudganEngine.Model;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Core;

public class TransactionsContainerTest
{
    public Mock<ILogger<TransactionsContainer>> LoggerMock { get; } = new();

    public string LayoutName { get; } = "test-layout";

    public string Origin { get; } = "Origin";

    public BankTransaction[] DummyTransactions { get; }
    
    public TransactionsContainer TransactionsContainer { get; }
    
    public TransactionsContainerTest()
    {
        DummyTransactions = new[]
        {
            new BankTransaction()
            {
                Key = "key1",
                Source = new BankTransactionSource()
                {
                    FileRelativePath = "Path",
                    InputId = "input-id"
                },
                LayoutName = LayoutName,
                DateTransaction = DateOnly.FromDateTime(DateTime.Now),
                DateInscription = DateOnly.FromDateTime(DateTime.Now),
                Amount = "100.0",
                Description = "Description",
                CardNumber = "12345"
            },
            new BankTransaction()
            {
            Key = "key2",
            Source = new BankTransactionSource()
            {
                FileRelativePath = "Path",
                InputId = "input-id"
            },
            LayoutName = LayoutName,
            DateTransaction = DateOnly.FromDateTime(DateTime.Now),
            DateInscription = DateOnly.FromDateTime(DateTime.Now),
            Amount = "200.0",
            Description = "Description",
            CardNumber = "12345"
            }
        };

        
        TransactionsContainer = new TransactionsContainer(
            LayoutName,
            Origin,
            LoggerMock.Object);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LayoutName, TransactionsContainer.LayoutName);
        Assert.Equal(Origin, TransactionsContainer.Origin);
    }

    [Fact]
    public void AddKeyNotPresent()
    {
        TransactionsContainer.Add(DummyTransactions[0]);

        Assert.Single(TransactionsContainer.Transactions);
        LoggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Never);
    }

    [Fact]
    public void AddKeyAlreadyPresent()
    {
        TransactionsContainer.Transactions.Add(DummyTransactions[0].Key, DummyTransactions[0]);
        TransactionsContainer.Add(DummyTransactions[0]);

        Assert.Single(TransactionsContainer.Transactions);
        LoggerMock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public void GetAllTransactions_Empty()
    {
        var result = TransactionsContainer.GetAllTransactions();

        Assert.Empty(result);
    }

    [Fact]
    public void GetAllTransactions_Filled()
    {
        foreach (var trans in DummyTransactions)
        {
            TransactionsContainer.Add(trans);
        }

        var result = TransactionsContainer.GetAllTransactions();

        Assert.Equal(2, result.Count());
    }
}