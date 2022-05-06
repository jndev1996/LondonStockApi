using FluentAssertions;
using LondonStockExchange.Api.Controllers;
using LondonStockExchange.Common.Models;
using LondonStockExchange.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LondonStockExchange.UnitTests.Controllers;

public class TradeControllerTests
{
    private readonly TradeController _tradeController;
    private readonly Mock<ITradeService> _tradeServiceMock;
    
    public TradeControllerTests()
    {
        _tradeServiceMock = new Mock<ITradeService>();
        _tradeController = new TradeController(_tradeServiceMock.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void CreateTrade_CreateTradeCommandTickerSymbolMissing_ReturnsBadRequest(string tickerSymbol)
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand
        {
            TickerSymbol = tickerSymbol,
            Price = 0,
            NumberOfShares = 1,
            BrokerId = 1,
        };

        //Act
        var result = await _tradeController.CreateTrade(createTradeCommand);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    public async void CreateTrade_CreateTradeCommandNumberOfSharesNotGreaterThanZero_ReturnsBadRequest(decimal numberOfShares)
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand
        {
            TickerSymbol = "asdf",
            Price = 0,
            NumberOfShares = numberOfShares,
            BrokerId = 1,
        };

        //Act
        var result = await _tradeController.CreateTrade(createTradeCommand);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [Fact]
    public async void CreateTrade_CreateTradeCommandPriceLessThanZero_ReturnsBadRequest()
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand
        {
            TickerSymbol = "asdf",
            Price = (decimal) -0.1,
            NumberOfShares = 1,
            BrokerId = 1,
        };

        //Act
        var result = await _tradeController.CreateTrade(createTradeCommand);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [Fact]
    public async void CreateTrade_CreateTradeCommandBrokerIdLessThanZero_ReturnsBadRequest()
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand
        {
            TickerSymbol = "asdf",
            Price = 0,
            NumberOfShares = 1,
            BrokerId = -1,
        };

        //Act
        var result = await _tradeController.CreateTrade(createTradeCommand);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }
    
    [Theory]
    [InlineData("12345")]
    [InlineData("123")]
    public async void CreateTrade_CreateTradeCommandTickerSymbolLengthNotEqualToFour_ReturnsBadRequest(string tickerSymbol)
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand
        {
            TickerSymbol = tickerSymbol,
            Price = 0,
            NumberOfShares = 1,
            BrokerId = 0,
        };

        //Act
        var result = await _tradeController.CreateTrade(createTradeCommand);

        //Assert
        result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public async void CreateTrade_ReturnsNoContent()
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand
        {
            TickerSymbol = "1234",
            Price = 0,
            NumberOfShares = 1,
            BrokerId = 1
        };
        
        //Act
        var result = await _tradeController.CreateTrade(createTradeCommand);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }
}