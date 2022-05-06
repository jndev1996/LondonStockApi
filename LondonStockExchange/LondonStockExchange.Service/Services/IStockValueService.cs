using LondonStockExchange.Common.Models;

namespace LondonStockExchange.Service.Services;

public interface IStockValueService
{
    public Task<decimal?> GetStockValuePriceByTickerSymbolAsync(string tickerSymbol);

    public Task<List<StockValue>> GetStockValuesByTickerSymbolsAsync(List<string> tickerSymbols);

    public Task<List<StockValue>> GetAllStockValuesAsync();
}