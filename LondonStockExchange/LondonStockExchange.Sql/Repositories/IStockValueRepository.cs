using LondonStockExchange.Common.Models;

namespace LondonStockExchange.Sql.Repositories;

public interface IStockValueRepository
{
    public Task<StockValue?> GetStockValueByTickerSymbolAsync(string tickerSymbol);
    public Task<List<StockValue>> GetAllStockValuesAsync();
    public Task<List<StockValue>> GetStockValuesByTickerSymbolsAsync(List<string> tickerSymbols);
}