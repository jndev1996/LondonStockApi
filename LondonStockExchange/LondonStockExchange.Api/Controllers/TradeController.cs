using LondonStockExchange.Common.Models;
using LondonStockExchange.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockExchange.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TradeController : ControllerBase
{
    private readonly ITradeService _tradeService;

    public TradeController(ITradeService tradeService)
    {
        _tradeService = tradeService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTrade([FromBody]CreateTradeCommand createTradeCommand)
    {
        if (string.IsNullOrWhiteSpace(createTradeCommand.TickerSymbol)
            || createTradeCommand.NumberOfShares <= 0
            || createTradeCommand.Price < 0
            || createTradeCommand.BrokerId < 0
            || createTradeCommand.TickerSymbol.Length != 4)
        {
            return BadRequest();
        }

        await _tradeService.CreateTradeAsync(createTradeCommand);
        return NoContent();
    }
}