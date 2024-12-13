using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.TradeDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService _tradeService;
        private readonly ILogger<TradeController> _logger;

        public TradeController(ITradeService tradeService, ILogger<TradeController> logger)
        {
            _tradeService = tradeService;
            _logger = logger;
        }

        // GET: api/trade/list
        [Authorize(Roles = "Admin,User")]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<TradeDTO>>> GetAllTrades()
        {
            _logger.LogInformation("Received request to GET all trades");

            try
            {
                var trades = await _tradeService.GetAllTradesAsync();

                if (!trades.Any())
                {
                    _logger.LogWarning("No trades found");
                }

                var tradeDtos = trades.Select(t => new TradeDTO
                {
                    TradeId = t.TradeId,
                    Account = t.Account,
                    AccountType = t.AccountType,
                    BuyQuantity = t.BuyQuantity,
                    SellQuantity = t.SellQuantity,
                    BuyPrice = t.BuyPrice,
                    SellPrice = t.SellPrice,
                    TradeDate = t.TradeDate,
                    TradeSecurity = t.TradeSecurity,
                    TradeStatus = t.TradeStatus,
                    Trader = t.Trader,
                    Benchmark = t.Benchmark,
                    Book = t.Book,
                    CreationName = t.CreationName,
                    CreationDate = t.CreationDate,
                    RevisionName = t.RevisionName,
                    RevisionDate = t.RevisionDate,
                    DealName = t.DealName,
                    DealType = t.DealType,
                    SourceListId = t.SourceListId,
                    Side = t.Side
                }).ToList();

                return Ok(tradeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all trades");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // POST: api/trade/add
        [Authorize(Roles = "Admin,User")]
        [HttpPost("add")]
        public async Task<IActionResult> AddTrade([FromBody] TradeCreateDTO tradeCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new trade");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for AddTrade request");
                    return BadRequest(ModelState);
                }

                var trade = new Trade
                {
                    Account = tradeCreateDto.Account,
                    AccountType = tradeCreateDto.AccountType,
                    BuyQuantity = tradeCreateDto.BuyQuantity,
                    SellQuantity = tradeCreateDto.SellQuantity,
                    BuyPrice = tradeCreateDto.BuyPrice,
                    SellPrice = tradeCreateDto.SellPrice,
                    TradeDate = tradeCreateDto.TradeDate,
                    TradeSecurity = tradeCreateDto.TradeSecurity,
                    TradeStatus = tradeCreateDto.TradeStatus,
                    Trader = tradeCreateDto.Trader,
                    Benchmark = tradeCreateDto.Benchmark,
                    Book = tradeCreateDto.Book,
                    CreationName = tradeCreateDto.CreationName,
                    RevisionName = tradeCreateDto.RevisionName,
                    DealName = tradeCreateDto.DealName,
                    DealType = tradeCreateDto.DealType,
                    SourceListId = tradeCreateDto.SourceListId,
                    Side = tradeCreateDto.Side
                };

                var createdTrade = await _tradeService.CreateTradeAsync(trade);

                _logger.LogInformation("Created trade with ID {TradeId}", createdTrade.TradeId);

                return CreatedAtAction(nameof(GetAllTrades), new { id = createdTrade.TradeId }, createdTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new trade");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // PUT: api/trade/update/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTrade(int id, [FromBody] TradeUpdateDTO tradeUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE trade with ID {TradeId}", id);

            try
            {
                var existingTrade = await _tradeService.GetTradeByIdAsync(id);
                if (existingTrade == null)
                {
                    _logger.LogWarning("Trade with ID {TradeId} not found for update", id);
                    return NotFound();
                }

                existingTrade.Account = tradeUpdateDto.Account;
                existingTrade.AccountType = tradeUpdateDto.AccountType;
                existingTrade.BuyQuantity = tradeUpdateDto.BuyQuantity;
                existingTrade.SellQuantity = tradeUpdateDto.SellQuantity;
                existingTrade.BuyPrice = tradeUpdateDto.BuyPrice;
                existingTrade.SellPrice = tradeUpdateDto.SellPrice;
                existingTrade.TradeDate = tradeUpdateDto.TradeDate;
                existingTrade.TradeSecurity = tradeUpdateDto.TradeSecurity;
                existingTrade.TradeStatus = tradeUpdateDto.TradeStatus;
                existingTrade.Trader = tradeUpdateDto.Trader;
                existingTrade.Benchmark = tradeUpdateDto.Benchmark;
                existingTrade.Book = tradeUpdateDto.Book;
                existingTrade.RevisionName = tradeUpdateDto.RevisionName;
                existingTrade.RevisionDate = tradeUpdateDto.RevisionDate;
                existingTrade.DealName = tradeUpdateDto.DealName;
                existingTrade.DealType = tradeUpdateDto.DealType;
                existingTrade.SourceListId = tradeUpdateDto.SourceListId;
                existingTrade.Side = tradeUpdateDto.Side;

                var updatedTrade = await _tradeService.UpdateTradeAsync(id, existingTrade);

                _logger.LogInformation("Updated trade with ID {TradeId}", id);

                return Ok(updatedTrade);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the trade with ID {TradeId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // DELETE: api/trade/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrade(int id)
        {
            _logger.LogInformation("Received request to DELETE trade with ID {TradeId}", id);

            try
            {
                var trade = await _tradeService.GetTradeByIdAsync(id);
                if (trade == null)
                {
                    _logger.LogWarning("Trade with ID {TradeId} not found for deletion", id);
                    return NotFound();
                }

                await _tradeService.DeleteTradeAsync(id);
                _logger.LogInformation("Deleted trade with ID {TradeId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the trade with ID {TradeId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}