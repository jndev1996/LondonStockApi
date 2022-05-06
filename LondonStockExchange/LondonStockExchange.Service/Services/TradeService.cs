using LondonStockExchange.Common.Models;
using LondonStockExchange.Sql.Repositories;

namespace LondonStockExchange.Service.Services;

public class TradeService : ITradeService
{
    private readonly ITradeRepository _tradeRepository;
    
    public TradeService(
        ITradeRepository tradeRepository)
    {
        _tradeRepository = tradeRepository;
    }

    public async Task CreateTradeAsync(CreateTradeCommand createTradeCommand)
    {
        if (string.IsNullOrWhiteSpace(createTradeCommand.TickerSymbol))
        {
            throw new ArgumentException("Create Trade Command Ticker Symbol is missing",
                nameof(createTradeCommand.TickerSymbol));
        }

        if (createTradeCommand.NumberOfShares <= 0)
        {
            throw new ArgumentException("Number of shares must be greater than zero",
                nameof(createTradeCommand.NumberOfShares));
        }
        
        if (createTradeCommand.Price < 0)
        {
            throw new ArgumentException("Price must be greater than or equal to zero",
                nameof(createTradeCommand.Price));
        }
        
        if (createTradeCommand.BrokerId < 0)
        {
            throw new ArgumentException("Broker Id must be greater than or equal to zero",
                nameof(createTradeCommand.BrokerId));
        }

        if (createTradeCommand.TickerSymbol.Length != 4)
        {
            throw new ArgumentException("Ticker Symbol must be 4 characters",
                nameof(createTradeCommand.TickerSymbol));
        }
        
        await _tradeRepository.CreateTrade(createTradeCommand.BrokerId, createTradeCommand.TickerSymbol,
            createTradeCommand.NumberOfShares, createTradeCommand.Price);
    }
}