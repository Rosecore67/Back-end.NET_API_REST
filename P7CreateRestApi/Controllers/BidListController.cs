using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs;
using P7CreateRestApi.Repositories.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BidListController : ControllerBase
    {
        private readonly IBidListRepository _bidListRepository;

        public BidListController(IBidListRepository bidListRepository)
        {
            _bidListRepository = bidListRepository;
        }

        // GET: api/bidlist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BidListDTO>>> GetAllBids()
        {
            var bids = await _bidListRepository.GetAllAsync();
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

        /*
        // POST: api/bidlist/validate
        //[HttpGet]
        //[Route("validate")]

        [HttpPost("validate")]
        public IActionResult Validate([FromBody] BidList bidList)
        {
            // TODO: check data valid and save to db, after saving return bid list
            return Ok();
        }

        Redondant avec le GET qui lui aussi vérifie les données avant d'enregistrer
        */

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

            await _bidListRepository.AddAsync(newBid);

            return CreatedAtAction(nameof(GetAllBids), new { id = newBid.BidListId }, newBid);
        }

        [HttpGet]
        [Route("update/{id}")]
        public IActionResult ShowUpdateForm(int id)
        {
            return Ok();
        }

        // PUT: api/bidlist/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBid(int id, [FromBody] BidListUpdateDTO bidUpdateDto)
        {
            var existingBid = await _bidListRepository.GetByIdAsync(id);
            if (existingBid == null) return NotFound();

            existingBid.Account = bidUpdateDto.Account;
            existingBid.BidType = bidUpdateDto.BidType;
            existingBid.BidQuantity = bidUpdateDto.BidQuantity;
            existingBid.AskQuantity = bidUpdateDto.AskQuantity;
            existingBid.Bid = bidUpdateDto.Bid;
            existingBid.Ask = bidUpdateDto.Ask;

            await _bidListRepository.UpdateAsync(existingBid);

            return NoContent();
        }

        // DELETE: api/bidlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBid(int id)
        {
            var bid = await _bidListRepository.GetByIdAsync(id);
            if (bid == null) return NotFound();

            await _bidListRepository.DeleteAsync(bid);
            return NoContent();
        }
    }
}