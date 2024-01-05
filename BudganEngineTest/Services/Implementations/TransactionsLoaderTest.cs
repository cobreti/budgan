using System.IO.Abstractions;
using BudganEngine.Model;
using BudganEngine.Services.Implementations;
using BudganEngine.Services.Interfaces;
using BudganEngineTest.Services.Implementations.MockedClasses;
using BudganEngineTest.Services.Implementations.MockedClasses.TransactionsLoader;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Services.Implementations;

public class TransactionsLoaderTest
{
    public Mock<ILogger<TransactionsLoader>> LoggerMock { get; } = new();

    public Mock<IFileSystem> FileSystemMock { get; } = new();

    public Mock<ITransactionParser> TransactionsParserMock { get; } = new();
    
    public Mock<IBankTransactionLayoutSettings> BankTransactionLayoutSettingsMock { get; } = new();
    
    public Mock<ICsvReaderFactory> CsvReaderFactoryMock { get; } = new();
    
    public TransactionsLoader TransactionsLoader { get; }

    public BankTransactionsLayout BankTransactionsLayout { get; } = new()
    {
        Name = "Layout",
        Amount = 0,
        DateTransaction = 1,
        DateInscription = 2,
        Description = 3,
        MinColumnsRequired = 2,
        Key = ["Amount", "DateTransaction", "DateInscription", "Description"]
    };
    
    public TransactionsLoaderTest()
    {
        TransactionsLoader = new TransactionsLoader(
            LoggerMock.Object,
            TransactionsParserMock.Object,
            CsvReaderFactoryMock.Object,
            BankTransactionLayoutSettingsMock.Object,
            FileSystemMock.Object);

        BankTransactionLayoutSettingsMock
            .Setup(x => x.GetByName(It.IsAny<string>()))
            .Returns(BankTransactionsLayout);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, TransactionsLoader.Logger);
        Assert.Equal(FileSystemMock.Object, TransactionsLoader.FileSystem);
        Assert.Equal(TransactionsParserMock.Object, TransactionsLoader.TransactionsParser);
        Assert.Equal(CsvReaderFactoryMock.Object, TransactionsLoader.CsvReaderFactory);
        Assert.Equal(BankTransactionLayoutSettingsMock.Object, TransactionsLoader.BankTransactionLayoutSettings);
    }

    [Fact]
    public void Load_PathDoesntExist()
    {
        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(false);
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);

        Assert.Throws<Exception>(() => TransactionsLoader.Load("key", "path", "layout"));
    }

    [Fact]
    public void Load_LayoutNotExists()
    {
        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);
        BankTransactionLayoutSettingsMock
            .Setup(x => x.GetByName(It.IsAny<string>()))
            .Returns<BankTransactionsLayout?>(null!);
        
        Assert.Throws<Exception>(() => TransactionsLoader.Load("key", "path", "layout"));
    }

    [Fact]
    public void Load_Directory()
    {
        var files = new string[] { "file1", "file2" };
            
        var fileMock = new Mock<IFile>();
        fileMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(false);
        FileSystemMock
            .SetupGet(x => x.File)
            .Returns(fileMock.Object);
        
        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);

        var directoryMock = new Mock<IDirectory>();
        directoryMock
            .Setup(x => x.GetFiles(It.IsAny<string>()))
            .Returns(files);
        directoryMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);

        FileSystemMock
            .SetupGet(x => x.Directory)
            .Returns(directoryMock.Object);

        pathMock
            .Setup(x => x.GetRelativePath(It.IsAny<string>(), It.Is<string>(x => x == "file1")))
            .Returns("relPathA");
        
        pathMock
            .Setup(x => x.GetRelativePath(It.IsAny<string>(), It.Is<string>(x => x == "file2")))
            .Returns("relPathB");
        
        var transactionsLoader = new TransactionsLoaderWithMockedReadFile(
            LoggerMock.Object,
            TransactionsParserMock.Object,
            CsvReaderFactoryMock.Object,
            BankTransactionLayoutSettingsMock.Object,
            FileSystemMock.Object);
        
        transactionsLoader.Load("key", "path", "layout");
        
        Assert.Equal(2, transactionsLoader.TransactionSources.Count);
        Assert.Equal("key", transactionsLoader.TransactionSources[0].InputId);
        Assert.Equal("key", transactionsLoader.TransactionSources[1].InputId);
        Assert.Equal("relPathA", transactionsLoader.TransactionSources[0].FileRelativePath);
        Assert.Equal("relPathB", transactionsLoader.TransactionSources[1].FileRelativePath);
        Assert.Equal(2, transactionsLoader.Files.Count);
        Assert.Equal("file1", transactionsLoader.Files[0]);
        Assert.Equal("file2", transactionsLoader.Files[1]);
        Assert.Equal(2, transactionsLoader.Layouts.Count);
        Assert.Equal(BankTransactionsLayout, transactionsLoader.Layouts[0]);
        Assert.Equal(BankTransactionsLayout, transactionsLoader.Layouts[1]);
        
    }

    [Fact]
    public void Load_File()
    {
        var fileMock = new Mock<IFile>();
        fileMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        FileSystemMock
            .SetupGet(x => x.File)
            .Returns(fileMock.Object);

        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);

        var directoryMock = new Mock<IDirectory>();
        directoryMock
            .Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(false);

        FileSystemMock
            .SetupGet(x => x.Directory)
            .Returns(directoryMock.Object);
        
        var transactionsLoader = new TransactionsLoaderWithMockedReadFile(
            LoggerMock.Object,
            TransactionsParserMock.Object,
            CsvReaderFactoryMock.Object,
            BankTransactionLayoutSettingsMock.Object,
            FileSystemMock.Object);

        transactionsLoader.Load("key", "path", "layout");

        Assert.Single(transactionsLoader.TransactionSources);
        Assert.Equal("key", transactionsLoader.TransactionSources[0].InputId);
        Assert.Equal("path", transactionsLoader.TransactionSources[0].FileRelativePath);
        Assert.Single(transactionsLoader.Files);
        Assert.Equal("path", transactionsLoader.Files[0]);
        Assert.Single(transactionsLoader.Layouts);
        Assert.Equal(BankTransactionsLayout, transactionsLoader.Layouts[0]);
    }
    
    [Fact]
    public void IsValidFile_LowercaseExtension()
    {
        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".csv");
        
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);
        
        Assert.True(TransactionsLoader.IsValidFile("file.csv"));
    }
    
    [Fact]
    public void IsValidFile_UppercaseExtension()
    {
        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".CSV");
        
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);
        
        Assert.True(TransactionsLoader.IsValidFile("file.CSV"));
    }

    [Fact]
    public void IsValidFile_InvalidExtension()
    {
        var pathMock = new Mock<IPath>();
        pathMock
            .Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".txt");
        
        FileSystemMock
            .SetupGet(x => x.Path)
            .Returns(pathMock.Object);
        
        Assert.False(TransactionsLoader.IsValidFile("file.txt"));
    }

    [Fact]
    public void ReadFile()
    {
        var transactionSource = new BankTransactionSource()
        {
            FileRelativePath = "file",
            InputId = "input-id"
        };
        
        var transactionsLoader = new TransactionsLoaderWithMockedValidFile (
            LoggerMock.Object,
            TransactionsParserMock.Object,
            CsvReaderFactoryMock.Object,
            BankTransactionLayoutSettingsMock.Object,
            FileSystemMock.Object);

        var csvReaderMock = new Mock<IReader>();

        CsvReaderFactoryMock
            .Setup(x => x.CreateFromFile(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(csvReaderMock.Object);
        
        transactionsLoader.ReadFile(transactionSource, "file", BankTransactionsLayout);
        
        TransactionsParserMock
            .Verify(x => x.Parse(transactionSource, "file", csvReaderMock.Object, BankTransactionsLayout), Times.Once);
    }

    [Fact]
    public void ReadFile_InvalidFile()
    {
        var transactionSource = new BankTransactionSource()
        {
            FileRelativePath = "file",
            InputId = "input-id"
        };

        var transactionsLoader = new TransactionsLoaderWithMockedValidFile (
            LoggerMock.Object,
            TransactionsParserMock.Object,
            CsvReaderFactoryMock.Object,
            BankTransactionLayoutSettingsMock.Object,
            FileSystemMock.Object);

        transactionsLoader.ValidFile = false;
        
        Assert.Throws<ArgumentException>(() => transactionsLoader.ReadFile(transactionSource, "file", BankTransactionsLayout));
    }
    
    [Fact]
    public void ReadFile_NullFile()
    {
        var transactionSource = new BankTransactionSource()
        {
            FileRelativePath = "file",
            InputId = "input-id"
        };

        var transactionsLoader = new TransactionsLoaderWithMockedValidFile (
            LoggerMock.Object,
            TransactionsParserMock.Object,
            CsvReaderFactoryMock.Object,
            BankTransactionLayoutSettingsMock.Object,
            FileSystemMock.Object);

        transactionsLoader.ValidFile = false;
        
        Assert.Throws<ArgumentException>(() => transactionsLoader.ReadFile(transactionSource, null!, BankTransactionsLayout));
    }
}