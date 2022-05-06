using LondonStockExchange.Common.Models;
using LondonStockExchange.Sql.Repositories;

namespace LondonStockExchange.Service.Services;

public class StockValueService : IStockValueService
{
    private readonly IStockValueRepository _stockValueRepository;
    
    public StockValueService(IStockValueRepository stockValueRepository)
    {
        _stockValueRepository = stockValueRepository;
    }

    public async Task<decimal?> GetStockValuePriceByTickerSymbolAsync(string tickerSymbol)
    {
        if (string.IsNullOrWhiteSpace(tickerSymbol))
        {
            throw new ArgumentException("Ticker Symbol cannot be null, empty or whitespace", nameof(tickerSymbol));
        }

        if (tickerSymbol.Length != 4)
        {
            return null;
        }

        return (await _stockValueRepository.GetStockValueByTickerSymbolAsync(tickerSymbol))?.Price;
    }

    public async Task<List<StockValue>> GetStockValuesByTickerSymbolsAsync(List<string> tickerSymbols)
    {
        if (tickerSymbols == null)
        {
            throw new ArgumentNullException(nameof(tickerSymbols), "Ticker Symbols list cannot be null");
        }

        if (!tickerSymbols.Any() || tickerSymbols.All(String.IsNullOrWhiteSpace) || tickerSymbols.All(ts => ts?.Length != 4))
        {
            return new List<StockValue>();
        }

        return await _stockValueRepository.GetStockValuesByTickerSymbolsAsync(tickerSymbols);
    }

    public async Task<List<StockValue>> GetAllStockValuesAsync()
    {
        return await _stockValueRepository.GetAllStockValuesAsync();
    }
}