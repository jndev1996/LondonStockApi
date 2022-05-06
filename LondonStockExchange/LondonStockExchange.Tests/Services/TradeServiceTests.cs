using FluentAssertions;
using LondonStockExchange.Common.Models;
using LondonStockExchange.Service.Services;
using LondonStockExchange.Sql.Repositories;
using Moq;
using Xunit;

namespace LondonStockExchange.UnitTests.Services;

public class TradeServiceTests
{
    private readonly TradeService _tradeService;
    private readonly Mock<ITradeRepository> _tradeRepositoryMock;
    
    public TradeServiceTests()
    {
        _tradeRepositoryMock = new Mock<ITradeRepository>();
        _tradeService = new TradeService(_tradeRepositoryMock.Object);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async void CreateTradeAsync_CreateTradeCommandTickerSymbolMissing_ThrowsArgumentException(string tickerSymbol)
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand()
        {
            Price = (decimal)10.1,
            BrokerId = 1,
            TickerSymbol = tickerSymbol,
            NumberOfShares = 11
        };

        //Act
        Func<Task> act = async () => await _tradeService.CreateTradeAsync(createTradeCommand);

        //Assert
        (await act.Should().ThrowAsync<ArgumentException>()).WithMessage("Create Trade Command Ticker Symbol is missing (Parameter 'TickerSymbol')");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.1)]
    public async void CreateTradeAsync_CreateTradeCommandNumberOfSharesNotGreaterThanZero_ThrowsArgumentException(int numberOfShares)
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand()
        {
            Price = (decimal)10.1,
            BrokerId = 1,
            TickerSymbol = "asdf",
            NumberOfShares = numberOfShares
        };

        //Act
        Func<Task> act = async () => await _tradeService.CreateTradeAsync(createTradeCommand);

        //Assert
        (await act.Should().ThrowAsync<ArgumentException>()).WithMessage("Number of shares must be greater than zero (Parameter 'NumberOfShares')");
    }
    
    [Fact]
    public async void CreateTradeAsync_CreateTradeCommandPriceNotGreaterThanZero_ThrowsArgumentException()
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand()
        {
            Price = (decimal)-0.1,
            BrokerId = 1,
            TickerSymbol = "asdf",
            NumberOfShares = 1
        };

        //Act
        Func<Task> act = async () => await _tradeService.CreateTradeAsync(createTradeCommand);

        //Assert
        (await act.Should().ThrowAsync<ArgumentException>()).WithMessage("Price must be greater than or equal to zero (Parameter 'Price')");
    }
    
    [Fact]
    public async void CreateTradeAsync_CreateTradeCommandBrokerIdNotGreaterThanZero_ThrowsArgumentException()
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand()
        {
            Price = (decimal)0.1,
            BrokerId = -1,
            TickerSymbol = "asdf",
            NumberOfShares = 1
        };

        //Act
        Func<Task> act = async () => await _tradeService.CreateTradeAsync(createTradeCommand);

        //Assert
        (await act.Should().ThrowAsync<ArgumentException>()).WithMessage("Broker Id must be greater than or equal to zero (Parameter 'BrokerId')");
    }
    
    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public async void CreateTradeAsync_TickerSymbolLengthNotFour_ThrowsArgumentException(string tickerSymbol)
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand()
        {
            Price = (decimal)0.1,
            BrokerId = 1,
            TickerSymbol = tickerSymbol,
            NumberOfShares = 1
        };

        //Act
        Func<Task> act = async () => await _tradeService.CreateTradeAsync(createTradeCommand);

        //Assert
        (await act.Should().ThrowAsync<ArgumentException>()).WithMessage("Ticker Symbol must be 4 characters (Parameter 'TickerSymbol')");
    }
    
    [Fact]
    public async void CreateTradeAsync_CallsRepoWithCreateTradeCommandFields()
    {
        //Arrange
        var createTradeCommand = new CreateTradeCommand()
        {
            Price = (decimal)0.1,
            BrokerId = 1,
            TickerSymbol = "asdf",
            NumberOfShares = 1
        };

        //Act
        await _tradeService.CreateTradeAsync(createTradeCommand);

        //Assert
        _tradeRepositoryMock.Verify(trm => trm.CreateTrade(createTradeCommand.BrokerId, createTradeCommand.TickerSymbol,
            createTradeCommand.NumberOfShares, createTradeCommand.Price), Times.Once);
        _tradeRepositoryMock.Verify(trm => trm.CreateTrade(It.IsAny<int>(), It.IsAny<string>(),
            It.IsAny<decimal>(), It.IsAny<decimal>()), Times.Once);
    }
}