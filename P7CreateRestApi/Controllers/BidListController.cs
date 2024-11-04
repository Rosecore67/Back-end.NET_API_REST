using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.BidListDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidListService _bidListService;

        public BidListController(IBidListService bidListService)
        {
            _bidListService = bidListService;
        }

        // GET: api/bidlist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BidListDTO>>> GetAllBids()
        {
            var bids = await _bidListService.GetAllBidsAsync();
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


        // POST: api/bidlist/validate
        [HttpPost("validate")]
        public async Task<IActionResult> CreateBid([FromBody] BidListCreateDTO bidCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

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

            return CreatedAtAction(nameof(GetAllBids), new { id = newBid.BidListId }, newBid);
        }


        // PUT: api/bidlist/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] BidListUpdateDTO bidUpdateDto)
        {
            var existingBid = await _bidListService.GetBidByIdAsync(id);
            if (existingBid == null) return NotFound();

            existingBid.Account = bidUpdateDto.Account;
            existingBid.BidType = bidUpdateDto.BidType;
            existingBid.BidQuantity = bidUpdateDto.BidQuantity;
            existingBid.AskQuantity = bidUpdateDto.AskQuantity;
            existingBid.Bid = bidUpdateDto.Bid;
            existingBid.Ask = bidUpdateDto.Ask;

            await _bidListService.UpdateBidAsync(id, existingBid);

            return Ok(existingBid);
        }

        // DELETE: api/bidlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var bid = await _bidListService.GetBidByIdAsync(id);
            if (bid == null) return NotFound();

            await _bidListService.DeleteBidAsync(id);
            return NoContent();
        }
    }
}