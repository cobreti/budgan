using BudganEngine.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Services.Implementations;

public class TransactionsContainerFactoryTest
{
    public Mock<ILogger<TransactionsContainerFactory>> LoggerMock { get; } = new();
    
    public Mock<ILoggerFactory> LoggerFactoryMock { get; } = new();
    
    public TransactionsContainerFactory TransactionsContainerFactory { get; }

    public TransactionsContainerFactoryTest()
    {
        TransactionsContainerFactory = new TransactionsContainerFactory(
            LoggerMock.Object,
            LoggerFactoryMock.Object);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, TransactionsContainerFactory.Logger);
        Assert.Equal(LoggerFactoryMock.Object, TransactionsContainerFactory.LoggerFactory);
    }

    [Fact]
    public void ContainerCreation()
    {
        var container = TransactionsContainerFactory.CreateTransactionsContainer("layout", "path");

        Assert.NotNull(container);
        Assert.Equal("layout", container.LayoutName);
        Assert.Equal("path", container.Origin);
    }
}