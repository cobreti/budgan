using BudganEngine.Services.Implementations;
using BudganEngine.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Services.Implementations;

public class TransactionsRepositoryTest
{
    public Mock<ILogger<TransactionsRepository>> LoggerMock { get; } = new();
    
    public Mock<ITransactionsContainerFactory> TransactionsContainerFactoryMock { get; } = new();
    
    public TransactionsRepository TransactionsRepository { get; }

    public TransactionsRepositoryTest()
    {
        TransactionsRepository = new TransactionsRepository(
            LoggerMock.Object,
            TransactionsContainerFactoryMock.Object);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, TransactionsRepository.Logger);
        Assert.Equal(TransactionsContainerFactoryMock.Object, TransactionsRepository.TransactionsContainerFactory);
    }
}
