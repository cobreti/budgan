using BudganEngine.Core;
using BudganEngine.Model;
using BudganEngine.Services.Implementations;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Services.Implementations;

public class TransactionsRepositoryTest
{
    public Mock<ILogger<TransactionsRepository>> LoggerMock { get; } = new();
    
    public Mock<ILogger<TransactionsContainer>> TransactionsContainerLoggerMock { get; } = new();
    
    public Mock<ITransactionsContainerFactory> TransactionsContainerFactoryMock { get; } = new();
    
    public TransactionsRepository TransactionsRepository { get; }
    
    public List<BankTransaction> BankTransactions { get; } = new();

    public List<TransactionsContainer> TransactionContainers { get; } = new();

    public TransactionsRepositoryTest()
    {
        TransactionsRepository = new TransactionsRepository(
            LoggerMock.Object,
            TransactionsContainerFactoryMock.Object);
        
        BankTransactions.Add( new BankTransaction()
        {
            Key = "key-1",
            LayoutName = "Layout-1",
            Amount = "2.00",
            CardNumber = "1234",
            DateInscription = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
            DateTransaction = DateOnly.FromDateTime(DateTime.Now),
            Description = "Description",
            Source = new BankTransactionSource()
            {
                FileRelativePath = "relPath",
                InputId = "inputId"
            }
        });
        BankTransactions.Add( new BankTransaction()
        {
            Key = "key-2",
            LayoutName = "Layout-1",
            Amount = "4.00",
            CardNumber = "12345",
            DateInscription = DateOnly.FromDateTime(DateTime.Now.AddDays(-3)),
            DateTransaction = DateOnly.FromDateTime(DateTime.Now),
            Description = "Description 2",
            Source = new BankTransactionSource()
            {
                FileRelativePath = "relPath",
                InputId = "inputId"
            }
        });
        BankTransactions.Add( new BankTransaction()
        {
            Key = "key-3",
            LayoutName = "Layout-2",
            Amount = "21.00",
            CardNumber = "23452",
            DateInscription = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            DateTransaction = DateOnly.FromDateTime(DateTime.Now),
            Description = "Description",
            Source = new BankTransactionSource()
            {
                FileRelativePath = "relPath-2",
                InputId = "inputId-2"
            }
        });
        BankTransactions.Add( new BankTransaction()
        {
            Key = "key-4",
            LayoutName = "Layout-2",
            Amount = "212.00",
            CardNumber = "23452",
            DateInscription = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
            DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
            Description = "Description",
            Source = new BankTransactionSource()
            {
                FileRelativePath = "relPath-2",
                InputId = "inputId-2"
            }
        });


        TransactionContainers.Add(new TransactionsContainer(
            BankTransactions[0].LayoutName,
            BankTransactions[0].Source.InputId,
            TransactionsContainerLoggerMock.Object));
        TransactionContainers.Add(new TransactionsContainer(
            BankTransactions[2].LayoutName,
            BankTransactions[2].Source.InputId,
            TransactionsContainerLoggerMock.Object));

        TransactionsContainerFactoryMock
            .Setup(x => x.CreateTransactionsContainer(BankTransactions[0].LayoutName, BankTransactions[0].Source.InputId))
            .Returns(TransactionContainers[0]);
        TransactionsContainerFactoryMock
            .Setup(x => x.CreateTransactionsContainer(BankTransactions[2].LayoutName, BankTransactions[2].Source.InputId))
            .Returns(TransactionContainers[1]);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, TransactionsRepository.Logger);
        Assert.Equal(TransactionsContainerFactoryMock.Object, TransactionsRepository.TransactionsContainerFactory);
    }

    [Fact]
    public void Add()
    {
        var transaction = BankTransactions[0];

        TransactionsRepository.Add(transaction);
        
        Assert.Single(TransactionsRepository.Containers);
        var container = TransactionsRepository.Containers[transaction.Source.InputId];

        Assert.NotNull(container);
        Assert.Equal(transaction.Source.InputId, container.Origin);

        var transactions = container.GetAllTransactions().ToList();

        Assert.Single(transactions);
        Assert.Equal(transaction, transactions[0]);
    }

    [Fact]
    public void GetContainerForTransaction_NewContainer()
    {
        var transaction = BankTransactions[0];
        
        var container = TransactionsRepository.GetContainerForTransaction(transaction);

        Assert.NotNull(container);
        Assert.Equal(transaction.Source.InputId, container.Origin);
        Assert.Equal(transaction.LayoutName, container.LayoutName);
        Assert.Single(TransactionsRepository.Containers);
        
        TransactionsContainerFactoryMock.Verify(
            x => x.CreateTransactionsContainer(transaction.LayoutName, transaction.Source.InputId),
            Times.Once);
    }

    [Fact]
    public void GetContainerForTransaction_ContainerAlreadyExists()
    {
        var transaction = BankTransactions[0];
        var container = TransactionContainers[0];
        TransactionsRepository.Containers.Add(container.Origin, container);
        
        var result = TransactionsRepository.GetContainerForTransaction(transaction);

        Assert.NotNull(result);
        Assert.Equal(container, result);
        TransactionsContainerFactoryMock.Verify(
            x => x.CreateTransactionsContainer(transaction.LayoutName, transaction.Source.InputId),
            Times.Never);
    }

    [Fact]
    public void GetAllTransactions()
    {
        TransactionsRepository.Containers.Add(TransactionContainers[0].Origin, TransactionContainers[0]);
        TransactionsRepository.Containers.Add(TransactionContainers[1].Origin, TransactionContainers[1]);
        
        BankTransactions.ForEach(x => TransactionsRepository.Add(x));
        
        var transactions = TransactionsRepository.GetAllTransactions().ToList();
        
        Assert.Equal(4, transactions.Count);
        Assert.Contains(BankTransactions[0], transactions);
        Assert.Contains(BankTransactions[1], transactions);
        Assert.Contains(BankTransactions[2], transactions);
        Assert.Contains(BankTransactions[3], transactions);
    }
}
