using System.Data;
using System.Data.SqlClient;
using LondonStockExchange.Common.Models;
using LondonStockExchange.Sql.Configuration;
using LondonStockExchange.Sql.Repositories.Models;
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
        var tickerSymbolCommandParameterName = "@Ticker";
        
        var sqlCommandParameter = new SqlCommandParameter
        {
            Name = tickerSymbolCommandParameterName,
            Value = tickerSymbol,
            SqlType = SqlDbType.NVarChar
        };
        
        var command = GetAllStockValuesCommand + 
            $" WHERE TickerSymbol = {tickerSymbolCommandParameterName}";

        return (await GetByCommandAsync<StockValue?>(command, 
            new List<SqlCommandParameter>{sqlCommandParameter})).FirstOrDefault();
    }

    public async Task<List<StockValue>> GetAllStockValuesAsync()
    {
        return await GetByCommandAsync<StockValue>(GetAllStockValuesCommand);
    }

    public async Task<List<StockValue>> GetStockValuesByTickerSymbolsAsync(List<string> tickerSymbols)
    {
        var command = GetAllStockValuesCommand + $" WHERE Tickers.TickerSymbol in (";

        var sqlCommandParameters = new List<SqlCommandParameter>();
        var sqlCommandParameterNameCounter = 0;
        
        foreach (var tickerSymbol in tickerSymbols)
        {
            var sqlCommandParameterName = $"@TickerSymbol{sqlCommandParameterNameCounter}";
            var sqlCommandParameter = new SqlCommandParameter
            {
                Name = sqlCommandParameterName,
                Value = tickerSymbol,
                SqlType = SqlDbType.NVarChar
            };

            command += sqlCommandParameterName;
            if (sqlCommandParameterNameCounter < tickerSymbols.Count -1)
            {
                command += ", ";
            }
            
            sqlCommandParameters.Add(sqlCommandParameter);
            sqlCommandParameterNameCounter++;
        }
        
        command += ");";

        return await GetByCommandAsync<StockValue>(command, sqlCommandParameters);
    }
}


