using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Services.Interface
{
    public interface IBidListService
    {
        Task<IEnumerable<BidList>> GetAllBidsAsync();
        Task<BidList> GetBidByIdAsync(int id);
        Task<BidList> CreateBidAsync(BidList bid);
        Task<BidList> UpdateBidAsync(int id, BidList bid);
        Task<bool> DeleteBidAsync(int id);
    }
}
