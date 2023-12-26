using System.Globalization;
using System.Text;
using BudganEngine.Extensions;
using BudganEngine.Model;
using Microsoft.Extensions.Logging;

namespace BudganEngineTest.Extensions;

public class DummyLogger : ILogger
{
    public string? LoggedMessage = null;
    
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        LoggedMessage = formatter(state, exception);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        throw new NotImplementedException();
    }
}

public class LoggerExtensionTest
{
    public BankTransaction DummyTransaction { get; } = new BankTransaction()
    {
        Key = "key1",
        Source = new BankTransactionSource()
        {
            FileRelativePath = "Path",
            InputId = "input-id"
        },
        LayoutName = "Layout",
        DateTransaction = DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
        DateInscription = DateOnly.FromDateTime(DateTime.Now),
        Amount = "100.0",
        Description = "Description",
        CardNumber = "12345"
    };

    public DummyLogger Logger { get; } = new();

    [Fact]
    public void ValidateExtensionOutput()
    {
        var message = "some message";
        var culture = CultureInfo.CurrentCulture;
        var dateInscription = DummyTransaction.DateInscription.ToString(culture);
        var dateTransaction = DummyTransaction.DateTransaction.ToString(culture);
        var expectedResult = new StringBuilder()
            .Append(message)
            .Append(" -- ")
            .Append(DummyTransaction.LayoutName)
            .Append("\t")
            .Append(DummyTransaction.Source.InputId)
            .Append("\t")
            .Append(DummyTransaction.Source.FileRelativePath)
            .Append("\t")
            .Append(dateTransaction)
            .Append("\t")
            .Append(dateInscription)
            .Append("\t")
            .Append(DummyTransaction.Amount)
            .Append("\t")
            .Append(DummyTransaction.Description)
            .Append("\t")
            .Append($"[{DummyTransaction.Key}]")
            .ToString();

        Logger.LogTransaction(message, DummyTransaction);

        Assert.Equal(expectedResult, Logger.LoggedMessage);
    }
    
    
}