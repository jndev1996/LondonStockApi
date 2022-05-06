using FluentAssertions;
using LondonStockExchange.Api.Controllers;
using LondonStockExchange.Common.Models;
using LondonStockExchange.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LondonStockExchange.UnitTests.Controllers;

public class StockValuesControllerTests
{
    private readonly Mock<IStockValueService> _stockValueServiceMock;
    private readonly StockValuesController _stockValuesController;
    
    public StockValuesControllerTests()
    {
        _stockValueServiceMock = new Mock<IStockValueService>();
        _stockValuesController = new StockValuesController(_stockValueServiceMock.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void GetByTickerSymbol_TickerSymbolIsMissing_ReturnsBadRequest(string tickerSymbol)
    {
        //Act
        var response = await _stockValuesController.GetByTickerSymbol(tickerSymbol);

        //Assert
        response.Should().BeOfType<BadRequestResult>();
    }
    
    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public async void GetByTickerSymbol_TickerSymbolIsNotFourCharactersInLength_ReturnsBadRequest(string tickerSymbol)
    {
        //Act
        var response = await _stockValuesController.GetByTickerSymbol(tickerSymbol);

        //Assert
        response.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async void GetByTickerSymbol_CallsStockValueServiceGetByTickerSymbol()
    {
        //Arrange
        var tickerSymbol = "asdf";
        
        //Act
        await _stockValuesController.GetByTickerSymbol(tickerSymbol);

        //Assert
        _stockValueServiceMock.Verify(svsm => svsm.GetStockValuePriceByTickerSymbolAsync(tickerSymbol), Times.Once);
        _stockValueServiceMock.Verify(svsm => svsm.GetStockValuePriceByTickerSymbolAsync(It.IsAny<string>()), Times.Once);
    }
    
    [Fact]
    public async void GetByTickerSymbol_StockValueServiceReturnsNull_ReturnsNotFound()
    {
        //Arrange
        var tickerSymbol = "asdf";
        _stockValueServiceMock.Setup(svsm => svsm.GetStockValuePriceByTickerSymbolAsync(tickerSymbol))
            .ReturnsAsync((decimal?) null);
        
        //Act
        var response = await _stockValuesController.GetByTickerSymbol(tickerSymbol);

        //Assert
        response.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async void GetByTickerSymbol_StockValueServiceReturnsValue_ReturnsOkWithValue()
    {
        //Arrange
        var responseValue = (decimal)10.2;
        
        var tickerSymbol = "asdf";
        _stockValueServiceMock.Setup(svsm => svsm.GetStockValuePriceByTickerSymbolAsync(tickerSymbol))
            .ReturnsAsync(responseValue);
        
        //Act
        var response = await _stockValuesController.GetByTickerSymbol(tickerSymbol);

        //Assert
        response.Should().BeOfType<OkObjectResult>();
        var result = (OkObjectResult) response;
        result.Value.Should().Be(responseValue);
    }

    [Fact]
    public async void GetByTickerSymbols_NoItemsInTickerSymbols_ReturnsBadRequest()
    {
        //Arrange
        var tickerSymbols = new List<string>();

        //Act
        var response = await _stockValuesController.GetByTickerSymbols(tickerSymbols);

        //Assert
        response.Should().BeOfType<BadRequestResult>();
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void GetByTickerSymbols_AllTickerSymbolsNullOrWhiteSpace_ReturnsBadRequest(string tickerSymbol)
    {
        //Arrange
        var tickerSymbols = new List<string>()
        {
            tickerSymbol,
            tickerSymbol,
            tickerSymbol
        };

        //Act
        var response = await _stockValuesController.GetByTickerSymbols(tickerSymbols);

        //Assert
        response.Should().BeOfType<BadRequestResult>();
    }
    
    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public async void GetByTickerSymbols_AllTickerSymbolsLengthNotFour_ReturnsNotFound(string tickerSymbol)
    {
        //Arrange
        var tickerSymbols = new List<string>()
        {
            tickerSymbol,
            tickerSymbol,
            tickerSymbol
        };

        //Act
        var response = await _stockValuesController.GetByTickerSymbols(tickerSymbols);

        //Assert
        response.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async void GetByTickerSymbols_CallsStockValueServiceGetStockValuesByTickers()
    {
        //Arrange
        var tickerSymbols = new List<string>()
        {
            "asdf"
        };
        
        _stockValueServiceMock.Setup(svsm => svsm.GetStockValuesByTickerSymbolsAsync(tickerSymbols))
            .ReturnsAsync(new List<StockValue>());

        //Act
        await _stockValuesController.GetByTickerSymbols(tickerSymbols);

        //Assert
        _stockValueServiceMock.Verify(svsm => svsm.GetStockValuesByTickerSymbolsAsync(tickerSymbols), Times.Once);
        _stockValueServiceMock.Verify(svsm => svsm.GetStockValuesByTickerSymbolsAsync(It.IsAny<List<string>>())
            , Times.Once);
    }
    
    [Fact]
    public async void GetByTickerSymbols_StockValuesServiceReturnsEmptyList_ReturnsNotFound()
    {
        //Arrange
        var tickerSymbols = new List<string>()
        {
            "asdf"
        };
        _stockValueServiceMock.Setup(svsm => svsm.GetStockValuesByTickerSymbolsAsync(tickerSymbols))
            .ReturnsAsync(new List<StockValue>());

        //Act
        var response = await _stockValuesController.GetByTickerSymbols(tickerSymbols);

        //Assert
        response.Should().BeOfType<NotFoundResult>();
    }
    
    [Fact]
    public async void GetByTickerSymbols_StockValuesServiceReturnsValues_OkResultWithValues()
    {
        //Arrange
        var tickerSymbols = new List<string>()
        {
            "asdf"
        };

        var expectedStockValues = new List<StockValue>()
        {
            new()
            {
                Price = 101,
                TickerSymbol = "ASDF"
            },
            new()
            {
                Price = 103,
                TickerSymbol = "TSLA"
            }
        };
        
        _stockValueServiceMock.Setup(svsm => svsm.GetStockValuesByTickerSymbolsAsync(tickerSymbols))
            .ReturnsAsync(expectedStockValues);

        //Act
        var response = await _stockValuesController.GetByTickerSymbols(tickerSymbols);

        //Assert
        response.Should().BeOfType<OkObjectResult>();
        var result = (OkObjectResult) response;
        result.Value.Should().BeEquivalentTo(expectedStockValues);
    }

    [Fact]
    public async void GetAll_CallsStockValueServiceGetAllStockValues()
    {
        //Arrange
        _stockValueServiceMock.Setup(svsm => svsm.GetAllStockValuesAsync()).ReturnsAsync(new List<StockValue>());
        
        //Act
        await _stockValuesController.GetAll();

        //Assert
        _stockValueServiceMock.Verify(svsm => svsm.GetAllStockValuesAsync(), Times.Once);
    }
    
    [Fact]
    public async void GetAll_StockValueServiceReturnsEmptyList_ReturnsNoContent()
    {
        //Arrange
        _stockValueServiceMock.Setup(svsm => svsm.GetAllStockValuesAsync()).ReturnsAsync(new List<StockValue>());
        
        //Act
        var response = await _stockValuesController.GetAll();

        //Assert
        response.Should().BeOfType<NoContentResult>();
    }
    
    [Fact]
    public async void GetAll_StockValueServiceReturnsPopulatedList_OkResultWithValues()
    {
        //Arrange
        var expectedStockValues = new List<StockValue>()
        {
            new()
            {
                Price = 1,
                TickerSymbol = "ASDF"
            }
        };
        
        _stockValueServiceMock.Setup(svsm => svsm.GetAllStockValuesAsync()).ReturnsAsync(expectedStockValues);
        
        //Act
        var response = await _stockValuesController.GetAll();

        //Assert
        response.Should().BeOfType<OkObjectResult>();
        var result = (OkObjectResult) response;
        result.Value.Should().BeEquivalentTo(expectedStockValues);
    }
}