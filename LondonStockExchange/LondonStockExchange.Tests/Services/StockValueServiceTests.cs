using LondonStockExchange.Service.Services;
using LondonStockExchange.Sql.Repositories;
using Moq;
using Xunit;
using FluentAssertions;
using LondonStockExchange.Common.Models;


namespace LondonStockExchange.UnitTests.Services;

public class StockValueServiceTests
{
    private readonly StockValueService _stockValueService;
    private readonly Mock<IStockValueRepository> _stockValueRepositoryMock;
    
    public StockValueServiceTests()
    {
        _stockValueRepositoryMock = new Mock<IStockValueRepository>();
        _stockValueService = new StockValueService(_stockValueRepositoryMock.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void GetStockValuePriceByTickerSymbolAsyncTickerSymbolMissing_ThrowsArgumentException(string tickerSymbol)
    {
        //Act
        Func<Task> act = async () => await _stockValueService.GetStockValuePriceByTickerSymbolAsync(tickerSymbol);

        //Assert
        (await act.Should().ThrowAsync<ArgumentException>()).WithMessage("Ticker Symbol cannot be null, empty or whitespace (Parameter 'tickerSymbol')");
    }
    
    [Fact]
    public async  void GetStockValuePriceByTickerSymbolAsyncTickerSymbolLengthGreaterThanFour_ReturnsNull()
    {
        //Act
        var result = await _stockValueService.GetStockValuePriceByTickerSymbolAsync("12345");

        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async void GetStockValuePriceByTickerSymbolAsyncTickerSymbolLengthLessThanFour_ReturnsNull()
    {
        //Act
        var result = await _stockValueService.GetStockValuePriceByTickerSymbolAsync("123");

        //Assert
        result.Should().BeNull();
    }
    
    [Fact]
    public async void GetStockValuePriceByTickerSymbolAsyncTickerSymbolPassesValidation_CallsRepo()
    {
        //Arrange
        var tickerSymbol = "asdf";
        
        //Act
        await _stockValueService.GetStockValuePriceByTickerSymbolAsync(tickerSymbol);

        //Assert
        _stockValueRepositoryMock.Verify(svrm => svrm.GetStockValueByTickerSymbolAsync(tickerSymbol), Times.Once);
        _stockValueRepositoryMock.Verify(svrm => svrm.GetStockValueByTickerSymbolAsync(It.IsAny<String>()), Times.Once);
    }
    
    [Fact]
    public async void GetStockValuePriceByTickerSymbolAsyncRepoReturnsStockValue_ReturnsStockValuePrice()
    {
        //Arrange
        var tickerSymbol = "asdf";
        var stockValue = new StockValue()
        {
            Price = (decimal)10.1,
            TickerSymbol = tickerSymbol
        };

        _stockValueRepositoryMock.Setup(svrm => svrm.GetStockValueByTickerSymbolAsync(tickerSymbol))
            .ReturnsAsync(stockValue);
        
        //Act
        var result = await _stockValueService.GetStockValuePriceByTickerSymbolAsync(tickerSymbol);

        //Assert
        result.Should().Be(stockValue.Price);
    }
    
    [Fact]
    public async void GetStockValuePriceByTickerSymbolAsyncRepoReturnsNull_ReturnsNull()
    {
        //Arrange
        var tickerSymbol = "asdf";

        _stockValueRepositoryMock.Setup(svrm => svrm.GetStockValueByTickerSymbolAsync(tickerSymbol))
            .ReturnsAsync((StockValue) null);
        
        //Act
        var result = await _stockValueService.GetStockValuePriceByTickerSymbolAsync(tickerSymbol);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async void GetStockValuesByTickerSymbolsAsync_TickerSymbolsNull_ThrowsArgumentNullException()
    {
        //Act
        Func<Task> act = async () => await _stockValueService.GetStockValuesByTickerSymbolsAsync(null);

        //Assert
        (await act.Should().ThrowAsync<ArgumentNullException>())
            .WithMessage("Ticker Symbols list cannot be null (Parameter 'tickerSymbols')");
    }

    [Fact]
    public async void GetStockValuesByTickerSymbolsAsync_TickerSymbolsListEmpty_ReturnsEmptyList()
    {
        //Act
        var result = await _stockValueService.GetStockValuesByTickerSymbolsAsync(new List<string>());
        
        //Assert
        result.Should().BeEquivalentTo(new List<string>());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("12345")]
    public async void GetStockValuesByTickerSymbolsAsync_TickerSymbolsDoNotPassValidation_ReturnsEmptyList(string tickerSymbol)
    {
        //Act
        var result = await _stockValueService.GetStockValuesByTickerSymbolsAsync(new List<string>()
        {
            tickerSymbol,
            tickerSymbol,
            tickerSymbol
        });
        
        //Assert
        result.Should().BeEquivalentTo(new List<string>());
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("12")]
    [InlineData("123")]
    [InlineData("12345")]
    public async void GetStockValuesByTickerSymbolsAsync_SomeTickerSymbolsDoNotPassValidation_CallsRepo(string tickerSymbol)
    {
        //Arrange
        var tickerSymbols = new List<string>
        {
            tickerSymbol,
            tickerSymbol,
            tickerSymbol,
            "1234"
        };
        
        //Act
        await _stockValueService.GetStockValuesByTickerSymbolsAsync(tickerSymbols);
        
        //Assert
        _stockValueRepositoryMock.Verify(svrm => svrm.GetStockValuesByTickerSymbolsAsync(tickerSymbols), Times.Once);
        _stockValueRepositoryMock.Verify(svrm => svrm.GetStockValuesByTickerSymbolsAsync(It.IsAny<List<string>>()), Times.Once);
    }
    
    [Fact]
    public async void GetStockValuesByTickerSymbolsAsync_RepoReturnsStockValues_ReturnsStockValues()
    {
        //Arrange

        var decimalValue = (decimal)10.32;

        var stockValues = new List<StockValue>
        {
            new()
            {
                Price = decimalValue,
                TickerSymbol = "asdf"
            },
            new()
            {
                Price = decimalValue,
                TickerSymbol = "asdf"
            }
        };
        
        _stockValueRepositoryMock.Setup(svrm => svrm.GetStockValuesByTickerSymbolsAsync(It.IsAny<List<string>>()))
            .ReturnsAsync(stockValues);
        
        //Act
        var result = await _stockValueService.GetStockValuesByTickerSymbolsAsync(new List<string>{"1234"});
        
        
        //Assert
        result.Should().BeEquivalentTo(stockValues);
    }

    [Fact]
    public async void GetAllStockValuesAsync_CallsRepo()
    {
        //Act
        await _stockValueService.GetAllStockValuesAsync();

        //Assert
        _stockValueRepositoryMock.Verify(svrm => svrm.GetAllStockValuesAsync(), Times.Once);
    }
    
    [Fact]
    public async void GetAllStockValuesAsync_RepoReturnsStockValues_ReturnsStockValues()
    {
        //Arrange
        var expectedResult = new List<StockValue>()
        {
            new()
            {
                Price = (decimal)12.2,
                TickerSymbol = "asdf"
            },
            new()
            {
                Price = (decimal)12.3,
                TickerSymbol = "asdg"
            }
        };

        _stockValueRepositoryMock.Setup(svrm => svrm.GetAllStockValuesAsync()).ReturnsAsync(expectedResult);
        
        //Act
        await _stockValueService.GetAllStockValuesAsync();

        //Assert
        _stockValueRepositoryMock.Verify(svrm => svrm.GetAllStockValuesAsync(), Times.Once);
    }
}