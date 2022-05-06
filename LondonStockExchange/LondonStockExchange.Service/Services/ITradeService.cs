using LondonStockExchange.Common.Models;

namespace LondonStockExchange.Service.Services;

public interface ITradeService
{
    public Task CreateTradeAsync(CreateTradeCommand createTradeCommand);
}