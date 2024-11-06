using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Services.Interface
{
    public interface ITradeService
    {
        Task<IEnumerable<Trade>> GetAllTradesAsync();
        Task<Trade> GetTradeByIdAsync(int id);
        Task<Trade> CreateTradeAsync(Trade trade);
        Task<Trade> UpdateTradeAsync(int id, Trade trade);
        Task<bool> DeleteTradeAsync(int id);
    }
}
