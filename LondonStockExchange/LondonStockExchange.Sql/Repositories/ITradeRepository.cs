namespace LondonStockExchange.Sql.Repositories;

public interface ITradeRepository
{
    public Task CreateTrade(int brokerId, string tickerSymbol, decimal numberOfShares, decimal price);
}