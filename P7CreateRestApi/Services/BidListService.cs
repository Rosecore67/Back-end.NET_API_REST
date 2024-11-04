using P7CreateRestApi.Services.Interface;
using P7CreateRestApi.Repositories.Interface;
using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Models.DTOs.BidListDTOs;

namespace P7CreateRestApi.Services
{
    public class BidListService : IBidListService
    {
        private readonly IBidListRepository _bidListRepository;

        public BidListService(IBidListRepository bidListRepository)
        {
            _bidListRepository = bidListRepository;
        }

        public async Task<IEnumerable<BidList>> GetAllBidsAsync()
        {
            return await _bidListRepository.GetAllAsync();
        }

        public async Task<BidList> GetBidByIdAsync(int id)
        {
            return await _bidListRepository.GetByIdAsync(id);
        }

        public async Task<BidList> CreateBidAsync(BidList bid)
        {
            await _bidListRepository.AddAsync(bid);
            return bid;
        }

        public async Task<BidList> UpdateBidAsync(int id, BidList bid)
        {
            var existingBid = await _bidListRepository.GetByIdAsync(id);
            if (existingBid == null) return null;
           
            existingBid.Account = bid.Account;
            existingBid.BidType = bid.BidType;
            existingBid.BidQuantity = bid.BidQuantity;
            existingBid.AskQuantity = bid.AskQuantity;
            existingBid.Bid = bid.Bid;
            existingBid.Ask = bid.Ask;
            existingBid.Benchmark = bid.Benchmark;

            await _bidListRepository.UpdateAsync(existingBid);
            return existingBid;
        }

        public async Task<bool> DeleteBidAsync(int id)
        {
            var bid = await _bidListRepository.GetByIdAsync(id);
            if (bid == null) return false;

            await _bidListRepository.DeleteAsync(bid);
            return true;
        }
    }
}
