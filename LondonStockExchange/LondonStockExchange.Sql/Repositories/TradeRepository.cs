using System.Data;
using LondonStockExchange.Sql.Configuration;
using LondonStockExchange.Sql.Repositories.Models;
using Microsoft.Extensions.Options;

namespace LondonStockExchange.Sql.Repositories;

public class TradeRepository : BaseRepository, ITradeRepository
{
    public TradeRepository(IOptions<SqlDatabaseConfiguration> sqlDatabaseConfiguration) : base(sqlDatabaseConfiguration){ }

    public async Task CreateTrade(int brokerId, string tickerSymbol, decimal numberOfShares, decimal price)
    {
	    var tickerSymbolName = "@TickerSymbolName";
	    var brokerIdName = "@BrokerIdName";
	    var numberOfSharesName = "@NumberOfSharesName";
	    var priceName = "@PriceName";
	    
	    var sqlCommandParameters = new List<SqlCommandParameter>
	    {
		    new()
		    {
			   Name = tickerSymbolName,
			   Value = tickerSymbol,
			   SqlType = SqlDbType.NVarChar
		    },
		    new()
		    {
			    Name = brokerIdName,
			    Value = brokerId,
			    SqlType = SqlDbType.Int
		    },
		    new()
		    {
			    Name = numberOfSharesName,
			    Value = numberOfShares,
			    SqlType = SqlDbType.Decimal
		    },
		    new()
		    {
			    Name = priceName,
			    Value = price,
			    SqlType = SqlDbType.Money
		    }
	    };
	    
        var command = $@"	BEGIN TRAN
							DECLARE @TradeGuid uniqueidentifier;
							DECLARE @TradesTable table (TradeGuid uniqueidentifier)
							DECLARE @TickerId int;					
       
                            IF NOT EXISTS 
				            (
					            SELECT * FROM  [dbo].[Tickers] Tickers
					            WITH (UPDLOCK, SERIALIZABLE)
					            WHERE Tickers.TickerSymbol = {tickerSymbolName}
				            )
				            BEGIN
				            INSERT INTO [dbo].[Tickers] ([TickerSymbol]) VALUES ({tickerSymbolName})	
				            END;

							SET @TickerId = (SELECT TickerId FROM [dbo].[Tickers] WHERE TickerSymbol = {tickerSymbolName})

                            IF NOT EXISTS 
                            (
	                            SELECT * FROM  [dbo].[Brokers] Brokers
	                            WITH (UPDLOCK, SERIALIZABLE)
	                            WHERE Brokers.BrokerId = {brokerIdName}
                            )
                            BEGIN
                            INSERT INTO [dbo].[Brokers] ([BrokerId]) VALUES ({brokerIdName})	
                            END;

                            INSERT INTO [dbo].[Trades]
                                       ([BrokerId]
                                       ,[TickerId]
                                       ,[Price]
                                       ,[NumberOfStocks])
								OUTPUT inserted.TradeGuid into @TradesTable
                                 VALUES
                                       ({brokerIdName}
                                       ,@TickerId
                                       ,{priceName}
                                       ,{numberOfSharesName})
							SELECT @TradeGuid = TradeGuid from @TradesTable

							UPDATE [dbo].[StockValues] WITH (UPDLOCK, SERIALIZABLE) SET TradeGuid = @TradeGuid
							WHERE TickerId = @TickerId AND
							(SELECT CreatedDate from Trades where TradeGuid = @TradeGuid) > (SELECT CreatedDate from Trades where TradeGuid = (select TradeGuid FROM StockValues where TickerId = @TickerId));
							 
							IF (@@ROWCOUNT = 0 AND NOT EXISTS (select * from StockValues WHERE TickerId = @TickerId))
							BEGIN
							  INSERT INTO [dbo].[StockValues](TickerId, TradeGuid) VALUES(@TickerId, @TradeGuid);
							END

                            COMMIT TRAN;";
        
        await ExecuteByCommandAsync(command, sqlCommandParameters);
    }
}