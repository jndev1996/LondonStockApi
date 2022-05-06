using LondonStockExchange.Common.Models;
using LondonStockExchange.Sql.Configuration;
using Microsoft.Extensions.Options;

namespace LondonStockExchange.Sql.Repositories;

public class StockValueRepository : BaseRepository, IStockValueRepository
{
    private const string GetAllStockValuesCommand =
        @"SELECT Tickers.TickerSymbol, Trades.Price FROM [dbo].[StockValues] StockValues
            JOIN [dbo].[Trades] Trades ON Trades.TradeGuid = StockValues.TradeGuid
            JOIN [dbo].[Tickers] Tickers ON Tickers.TickerId = StockValues.TickerId";
    
    public StockValueRepository(IOptions<SqlDatabaseConfiguration> sqlDatabaseConfiguration) : base(sqlDatabaseConfiguration){ }

    public async Task<StockValue?> GetStockValueByTickerSymbolAsync(string tickerSymbol)
    {
        var command = GetAllStockValuesCommand + 
            $" WHERE TickerSymbol = '{tickerSymbol}'";

        return (await GetByCommandAsync<StockValue?>(command)).FirstOrDefault();
    }

    public async Task<List<StockValue>> GetAllStockValuesAsync()
    {
        return await GetByCommandAsync<StockValue>(GetAllStockValuesCommand);
    }

    public async Task<List<StockValue>> GetStockValuesByTickerSymbolsAsync(List<string> tickerSymbols)
    {
        //Strip tickerSymbols of Sql interrupting stuff
        var sqlInValues = string.Join(",", tickerSymbols);
        sqlInValues = string.Join(",", sqlInValues.Split(',').Select(x => string.Format("'{0}'", x)).ToList());
        var command = GetAllStockValuesCommand + $" WHERE Tickers.TickerSymbol in ({sqlInValues})";

        return await GetByCommandAsync<StockValue>(command);
    }
}


