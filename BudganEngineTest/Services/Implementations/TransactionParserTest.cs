using System.Globalization;
using System.Text;
using BudganEngine.Model;
using BudganEngine.Options;
using BudganEngine.Services.Implementations;
using BudganEngine.Services.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BudganEngineTest.Services.Implementations;


public class TransactionParser_ParseRowMocked : TransactionParser
{
    public int ParseRowCount { get; set; } = 0;
    
    public TransactionParser_ParseRowMocked(
        ILogger<TransactionParser> logger, 
        IOptions<AppConfig> appConfigOptions, 
        ITransactionsRepository transactionsRepository) : base(logger, appConfigOptions, transactionsRepository)
    {
    }

    public override void ParseRow(BankTransactionSource transactionSource, IParser parser, BankTransactionsLayout layout)
    {
        ParseRowCount++;
    }
}


public class TransactionParserTest
{
    public Mock<ILogger<TransactionParser>> LoggerMock { get; } = new();

    public Mock<ITransactionsRepository> TransactionsRepositoryMock { get; } = new();

    public Mock<IOptions<AppConfig>> AppConfigOptionsMock { get; } = new();
    
    // public Mock<ICsvReaderFactory> CsvReaderFactoryMock { get; } = new();
    
    public TransactionParser TransactionParser { get; }
    
    // public StreamReader CsvStreamReader { get; set; }
    
    // public CsvConfiguration CsvReaderConfig { get; set; }
    
    // public CsvReader? CsvReader { get; set; }
    
    public BankTransactionsLayout Layout { get; }
    
    public BankTransactionSource TransactionSource { get; } = new()
    {
        InputId = "1",
        FileRelativePath = "test"
    };
    
    public TransactionParserTest()
    {
        Layout = new BankTransactionsLayout
        {
            MinColumnsRequired = 2,
            Name = "test",
            CardNumber = 0,
            DateTransaction = 1,
            DateInscription = 2,
            Amount = 3,
            Description = 4,
            Key = new []{ "CardNumber", "DateTransaction", "Amount", "Description" }
        };
        
        AppConfigOptionsMock.SetupGet(x => x.Value).Returns(new AppConfig
        {
            DateFormat = "dd/MM/yyyy"
        });
        
        // CsvReaderFactoryMock
        //     .Setup(x => x.CreateReader(It.IsAny<StreamReader>(), It.IsAny<CsvConfiguration>()))
        //     .Callback<StreamReader, CsvConfiguration>((reader, configuration) =>
        //     {
        //         Console.WriteLine("before returns");
        //         CsvStreamReader = reader;
        //         CsvReaderConfig = configuration;
        //     })
        //     .Returns(() =>
        //     {
        //         CsvReader = new CsvReader(CsvStreamReader, CsvReaderConfig);
        //         return CsvReader;
        //     });

        TransactionParser = new TransactionParser(
            LoggerMock.Object,
            AppConfigOptionsMock.Object,
            // CsvReaderFactoryMock.Object,
            TransactionsRepositoryMock.Object);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, TransactionParser.Logger);
        Assert.Equal( TransactionsRepositoryMock.Object, TransactionParser.TransactionsRepository);
        Assert.Equal( AppConfigOptionsMock.Object, TransactionParser.AppConfigOptions);
        Assert.Equal("dd/MM/yyyy", TransactionParser.DateFormat);
    }

    [Fact]
    public void Parse()
    {
        var csvContent = new StringBuilder()
            .Append("\r\n")
            .Append("CardNumber,DateTransaction,DateInscription,Amount,Description\r\n")
            .Append("1234567890123456,01/01/2021,02/01/2021,100,test\r\n")
            .Append("1234761590123456,02/08/2020,09/02/2021,100,test\r\n")
            .ToString();
        
        using var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvContent)));
        using var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture);
        
        var transactionParser = new TransactionParser_ParseRowMocked(
            LoggerMock.Object,
            AppConfigOptionsMock.Object,
            // CsvReaderFactoryMock.Object,
            TransactionsRepositoryMock.Object);
        
        transactionParser.Parse(TransactionSource, "test", csvReader, Layout);

        Assert.NotNull(csvReader);
        Assert.NotNull(csvReader.HeaderRecord);
        Assert.Equal(5, csvReader.HeaderRecord.Length);
        Assert.Equal("CardNumber", csvReader.HeaderRecord[0]);
        Assert.Equal("DateTransaction", csvReader.HeaderRecord[1]);
        Assert.Equal("DateInscription", csvReader.HeaderRecord[2]);
        Assert.Equal("Amount", csvReader.HeaderRecord[3]);
        Assert.Equal("Description", csvReader.HeaderRecord[4]);
        Assert.Equal(2, transactionParser.ParseRowCount);
    }

    [Fact]
    public void ParseRow()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            AllowComments = true,
            ShouldSkipRecord = (args) => args.Row.Parser.Count < 2
        };
        
        var csvContent = new StringBuilder()
            .Append("1234567890123456,01/01/2021,02/01/2021,100,test\r\n")
            .ToString();
        
        var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvContent)));

        var csvReader = new CsvReader(streamReader, config);

        BankTransaction? addedTransaction = null;
        TransactionsRepositoryMock
            .Setup(x => x.Add(It.IsAny<BankTransaction>()))
            .Callback<BankTransaction>(transaction => addedTransaction = transaction);
        
        csvReader.Read();
        
        TransactionParser.ParseRow(TransactionSource, csvReader.Parser, Layout);

        Assert.NotNull(addedTransaction);
        Assert.Equal("1234567890123456", addedTransaction.CardNumber);
        Assert.Equal(new DateOnly(2021, 01, 01), addedTransaction.DateTransaction);
        Assert.Equal(new DateOnly(2021, 01, 02), addedTransaction.DateInscription);
        Assert.Equal("100", addedTransaction.Amount);
        Assert.Equal("test", addedTransaction.Description);
        Assert.Equal("123456789012345601/01/2021100test", addedTransaction.Key);
    }
    
    [Fact]
    public void ParseRow_WithInvalidDate()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            AllowComments = true,
            ShouldSkipRecord = (args) => args.Row.Parser.Count < 2
        };
        
        var csvContent = new StringBuilder()
            .Append("1234567890123456,01/16/2021,02/01/2021,100,test\r\n")
            .ToString();
        
        var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvContent)));

        var csvReader = new CsvReader(streamReader, config);

        BankTransaction? addedTransaction = null;
        TransactionsRepositoryMock
            .Setup(x => x.Add(It.IsAny<BankTransaction>()))
            .Callback<BankTransaction>(transaction => addedTransaction = transaction);
        
        csvReader.Read();

        TransactionParser.ParseRow(TransactionSource, csvReader.Parser, Layout);
        
        LoggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Once);
    }

    [Fact]
    public void GetColumnValueWithNullColumn()
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            AllowComments = true,
            ShouldSkipRecord = (args) => args.Row.Parser.Count < 2
        };
        
        var csvContent = new StringBuilder()
            .Append("1234567890123456,01/16/2021,02/01/2021,100,test\r\n")
            .ToString();
        
        var streamReader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(csvContent)));

        var csvReader = new CsvReader(streamReader, config);

        var res = TransactionParser.GetColumnValue(csvReader.Parser,null);

        Assert.Equal("", res);
    }
}
