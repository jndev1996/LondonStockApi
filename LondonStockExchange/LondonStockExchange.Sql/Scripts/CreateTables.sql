Create Table [dbo].[Brokers](
    BrokerId [int] NOT NULL
    PRIMARY KEY(BrokerId)
    )

Create Table [dbo].[Tickers](
    TickerId [int] NOT NULL IDENTITY,
    TickerSymbol VARCHAR(4) NOT NULL
    PRIMARY KEY(TickerId),
    UNIQUE(TickerSymbol)
    )

Create Table [dbo].[Trades](
    TradeGuid [uniqueidentifier] NOT NULL DEFAULT (newsequentialid()),
    BrokerId [int] NOT NULL,
    TickerId [int] NOT NULL,
    Price [money] NOT NULL,
    NumberOfStocks [decimal] NOT NULL,
    CreatedDate [dateTime] NOT NULL DEFAULT(CURRENT_TIMESTAMP)
    PRIMARY KEY(TradeGuid),
    FOREIGN KEY(BrokerId) REFERENCES [dbo].[Brokers](BrokerId),
    FOREIGN KEY(TickerId) REFERENCES [dbo].[Tickers](TickerId)
    )

Create Table [dbo].[StockValues](
    TickerId [int] NOT NULL,
    TradeGuid [uniqueidentifier]
    PRIMARY KEY (TickerId),
    FOREIGN KEY (TickerId) REFERENCES [dbo].[Tickers](TickerId),
    FOREIGN KEY (TradeGuid) REFERENCES [dbo].[Trades](TradeGuid)
    )