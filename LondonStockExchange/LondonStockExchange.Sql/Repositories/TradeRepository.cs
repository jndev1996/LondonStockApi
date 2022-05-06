using LondonStockExchange.Sql.Configuration;
using Microsoft.Extensions.Options;

namespace LondonStockExchange.Sql.Repositories;

public class TradeRepository : BaseRepository, ITradeRepository
{
    public TradeRepository(IOptions<SqlDatabaseConfiguration> sqlDatabaseConfiguration) : base(sqlDatabaseConfiguration){ }

    public async Task CreateTrade(int brokerId, string tickerSymbol, decimal numberOfShares, decimal price)
    {
        var command = $@"	BEGIN TRAN
							DECLARE @TradeGuid uniqueidentifier;
							DECLARE @TradesTable table (TradeGuid uniqueidentifier)
							DECLARE @TickerId int;					
       
                            IF NOT EXISTS 
				            (
					            SELECT * FROM  [dbo].[Tickers] Tickers
					            WITH (UPDLOCK, SERIALIZABLE)
					            WHERE Tickers.TickerSymbol = '{tickerSymbol}'
				            )
				            BEGIN
				            INSERT INTO [dbo].[Tickers] ([TickerSymbol]) VALUES ('{tickerSymbol}')	
				            END;

							SET @TickerId = (SELECT TickerId FROM [dbo].[Tickers] WHERE TickerSymbol = '{tickerSymbol}')

                            IF NOT EXISTS 
                            (
	                            SELECT * FROM  [dbo].[Brokers] Brokers
	                            WITH (UPDLOCK, SERIALIZABLE)
	                            WHERE Brokers.BrokerId = {brokerId}
                            )
                            BEGIN
                            INSERT INTO [dbo].[Brokers] ([BrokerId]) VALUES ({brokerId})	
                            END;

                            INSERT INTO [dbo].[Trades]
                                       ([BrokerId]
                                       ,[TickerId]
                                       ,[Price]
                                       ,[NumberOfStocks])
								OUTPUT inserted.TradeGuid into @TradesTable
                                 VALUES
                                       ({brokerId}
                                       ,@TickerId
                                       ,{price}
                                       ,{numberOfShares})
							SELECT @TradeGuid = TradeGuid from @TradesTable

							UPDATE [dbo].[StockValues] WITH (UPDLOCK, SERIALIZABLE) SET TradeGuid = @TradeGuid
							WHERE TickerId = @TickerId AND
							(SELECT CreatedDate from Trades where TradeGuid = @TradeGuid) > (SELECT CreatedDate from Trades where TradeGuid = (select TradeGuid FROM StockValues where TickerId = @TickerId));
							 
							IF (@@ROWCOUNT = 0 AND NOT EXISTS (select * from StockValues WHERE TickerId = @TickerId))
							BEGIN
							  INSERT INTO [dbo].[StockValues](TickerId, TradeGuid) VALUES(@TickerId, @TradeGuid);
							END

                            COMMIT TRAN;";
        
        await ExecuteByCommandAsync(command);
    }
}