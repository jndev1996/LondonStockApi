namespace LondonStockExchange.Common.Models;

public class CreateTradeCommand
{
    public int BrokerId { get; set; }
    public string TickerSymbol { get; set; }
    public decimal NumberOfShares  { get; set; }
    public decimal Price { get; set; }
}