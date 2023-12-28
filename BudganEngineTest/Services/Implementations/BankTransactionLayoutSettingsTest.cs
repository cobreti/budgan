using BudganEngine.Model;
using BudganEngine.Services.Implementations;
using Microsoft.Extensions.Logging;
using Moq;

namespace BudganEngineTest.Services.Implementations;

public class BankTransactionLayoutSettingsTest
{
    public Mock<ILogger<BankTransactionLayoutSettings>> LoggerMock { get; set; } = new();

    public BankTransactionLayoutSettings LayoutSettings { get; }
    
    public BankTransactionLayoutSettingsTest()
    {
        LayoutSettings = new BankTransactionLayoutSettings(LoggerMock.Object);
    }

    [Fact]
    public void Construction()
    {
        Assert.Equal(LoggerMock.Object, LayoutSettings.Logger);
    }
    
    [Fact]
    public void AddOrReplace_AddNewLayout_LayoutAdded()
    {
        // Arrange
        var layout = new BankTransactionsLayout
        {
            Name = "TestLayout"
        };
        
        // Act
        LayoutSettings.AddOrReplace(layout);
        
        // Assert
        Assert.Equal(layout, LayoutSettings.Layouts[layout.Name]);
    }
    
    [Fact]
    public void AddOrReplace_ReplaceExistingLayout_LayoutReplaced()
    {
        // Arrange
        var layout = new BankTransactionsLayout
        {
            Name = "TestLayout"
        };
        LayoutSettings.AddOrReplace(layout);
        
        var newLayout = new BankTransactionsLayout
        {
            Name = "TestLayout"
        };
        
        // Act
        LayoutSettings.AddOrReplace(newLayout);
        
        // Assert
        Assert.Equal(newLayout, LayoutSettings.Layouts[newLayout.Name]);
    }
    
    [Fact]
    public void GetByName_LayoutExists_ReturnLayout()
    {
        // Arrange
        var layout = new BankTransactionsLayout
        {
            Name = "TestLayout"
        };
        LayoutSettings.AddOrReplace(layout);
        
        // Act
        var result = LayoutSettings.GetByName(layout.Name);
        
        // Assert
        Assert.Equal(layout, result);
    }
}