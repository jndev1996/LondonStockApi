using LondonStockExchange.Common.Models;
using LondonStockExchange.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace LondonStockExchange.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StockValuesController : ControllerBase
{
    private readonly IStockValueService _stockValueService;

    public StockValuesController(IStockValueService stockValueService)
    {
        _stockValueService = stockValueService;
    }

    [HttpGet]
    [Route("{tickerSymbol}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(decimal))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByTickerSymbol(string tickerSymbol)
    {
        if (string.IsNullOrWhiteSpace(tickerSymbol))
        {
            return BadRequest();
        }

        if (tickerSymbol.Length != 4)
        {
            return NotFound();
        }

        var result = await _stockValueService.GetStockValuePriceByTickerSymbolAsync(tickerSymbol);

        if (result != null)
        {
            return Ok(result.Value);
        }

        return NotFound();
    }

    [HttpGet]
    [Route("tickerSymbols")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockValue>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByTickerSymbols([FromQuery] List<string> tickerSymbols)
    {
        if (!tickerSymbols.Any() || tickerSymbols.All(String.IsNullOrWhiteSpace))
        {
            return BadRequest();
        }

        if (tickerSymbols.All(ts => ts.Length != 4))
        {
            return NotFound();
        }
        
        var stockValues = await _stockValueService.GetStockValuesByTickerSymbolsAsync(tickerSymbols);
        
        if (stockValues.Any())
        {
            return Ok(stockValues);    
        }

        return NotFound();
    }
    
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockValue>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAll()
    {
        var stockValues = await _stockValueService.GetAllStockValuesAsync();

        if (!stockValues.Any())
        {
            return NoContent();
        }
        
        return Ok(stockValues);
    }
}