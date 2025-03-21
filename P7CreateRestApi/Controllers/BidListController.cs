using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.BidListDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidListService _bidListService;
        private readonly ILogger<BidListController> _logger;

        public BidListController(IBidListService bidListService, ILogger<BidListController> logger)
        {
            _bidListService = bidListService;
            _logger = logger;
        }

        // GET: api/bidlist/list
        [Authorize(Roles = "Admin")]
        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<BidListDTO>>> GetAllBids()
        {
            _logger.LogInformation("Received request to GET all bids at {Time}", DateTime.Now);

            try
            {
                var bids = await _bidListService.GetAllBidsAsync();

                if (!bids.Any())
                {
                    _logger.LogWarning("No bids found at {Time}", DateTime.Now);
                }
                else
                {
                    _logger.LogInformation("Fetched {Count} bids at {Time}", bids.Count(), DateTime.Now);
                }

                var bidDtos = bids.Select(bid => new BidListDTO
                {
                    BidListId = bid.BidListId,
                    Account = bid.Account,
                    BidType = bid.BidType,
                    BidQuantity = bid.BidQuantity,
                    AskQuantity = bid.AskQuantity,
                    Bid = bid.Bid,
                    Ask = bid.Ask,
                    Benchmark = bid.Benchmark,
                    Commentary = bid.Commentary,
                    BidStatus = bid.BidStatus,
                    CreationDate = bid.CreationDate,
                }).ToList();

                return Ok(bidDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all bids at {Time}", DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }


        // POST: api/bidlist/add
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> CreateBid([FromBody] BidListCreateDTO bidCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new bid at {Time}", DateTime.Now);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for CreateBid request at {Time}", DateTime.Now);
                    return BadRequest(ModelState);
                }

                var newBid = new BidList
                {
                    Account = bidCreateDto.Account,
                    BidType = bidCreateDto.BidType,
                    BidQuantity = bidCreateDto.BidQuantity,
                    AskQuantity = bidCreateDto.AskQuantity,
                    Bid = bidCreateDto.Bid,
                    Ask = bidCreateDto.Ask,
                    Benchmark = bidCreateDto.Benchmark,
                    Commentary = bidCreateDto.Commentary,
                    BidSecurity = bidCreateDto.BidSecurity,
                    BidStatus = bidCreateDto.BidStatus,
                    Trader = bidCreateDto.Trader,
                    Book = bidCreateDto.Book,
                    CreationName = bidCreateDto.CreationName,
                    RevisionName = bidCreateDto.RevisionName,
                    DealName = bidCreateDto.DealName,
                    DealType = bidCreateDto.DealType,
                    SourceListId = bidCreateDto.SourceListId,
                    Side = bidCreateDto.Side,
                    CreationDate = DateTime.Now
                };

                await _bidListService.CreateBidAsync(newBid);

                _logger.LogInformation("Created bid with ID {BidId} at {Time}", newBid.BidListId, DateTime.Now);

                return CreatedAtAction(nameof(GetAllBids), new { id = newBid.BidListId }, newBid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new bid at {Time}", DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }


        // PUT: api/bidlist/update/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] BidListUpdateDTO bidUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE bid with ID {BidId} at {Time}", id, DateTime.Now);

            try
            {
                var existingBid = await _bidListService.GetBidByIdAsync(id);
                if (existingBid == null)
                {
                    _logger.LogWarning("Bid with ID {BidId} not found for update at {Time}", id, DateTime.Now);
                    return NotFound();
                }

                existingBid.Account = bidUpdateDto.Account;
                existingBid.BidType = bidUpdateDto.BidType;
                existingBid.BidQuantity = bidUpdateDto.BidQuantity;
                existingBid.AskQuantity = bidUpdateDto.AskQuantity;
                existingBid.Bid = bidUpdateDto.Bid;
                existingBid.Ask = bidUpdateDto.Ask;

                await _bidListService.UpdateBidAsync(id, existingBid);

                _logger.LogInformation("Updated bid with ID {BidId} at {Time}", id, DateTime.Now);

                return Ok(existingBid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the bid with ID {BidId} at {Time}", id, DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // DELETE: api/bidlist/delete/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            _logger.LogInformation("Received request to DELETE bid with ID {BidId} at {Time}", id, DateTime.Now);

            try
            {
                var bid = await _bidListService.GetBidByIdAsync(id);
                if (bid == null)
                {
                    _logger.LogWarning("Bid with ID {BidId} not found for deletion at {Time}", id, DateTime.Now);
                    return NotFound();
                }

                await _bidListService.DeleteBidAsync(id);
                _logger.LogInformation("Deleted bid with ID {BidId} at {Time}", id, DateTime.Now);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the bid with ID {BidId} at {Time}", id, DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}