using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Repositories.Interface;
using P7CreateRestApi.Services.Interface;

namespace P7CreateRestApi.Services
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;

        public TradeService(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }

        public async Task<IEnumerable<Trade>> GetAllTradesAsync()
        {
            return await _tradeRepository.GetAllAsync();
        }

        public async Task<Trade> GetTradeByIdAsync(int id)
        {
            return await _tradeRepository.GetByIdAsync(id);
        }

        public async Task<Trade> CreateTradeAsync(Trade trade)
        {
            await _tradeRepository.AddAsync(trade);
            return trade;
        }

        public async Task<Trade> UpdateTradeAsync(int id, Trade trade)
        {
            var existingTrade = await _tradeRepository.GetByIdAsync(id);
            if (existingTrade == null) return null;

            existingTrade.Account = trade.Account;
            existingTrade.AccountType = trade.AccountType;
            existingTrade.BuyQuantity = trade.BuyQuantity;
            existingTrade.SellQuantity = trade.SellQuantity;
            existingTrade.BuyPrice = trade.BuyPrice;
            existingTrade.SellPrice = trade.SellPrice;
            existingTrade.TradeDate = trade.TradeDate;
            existingTrade.TradeSecurity = trade.TradeSecurity;
            existingTrade.TradeStatus = trade.TradeStatus;
            existingTrade.Trader = trade.Trader;
            existingTrade.Benchmark = trade.Benchmark;
            existingTrade.Book = trade.Book;
            existingTrade.CreationName = trade.CreationName;
            existingTrade.RevisionName = trade.RevisionName;
            existingTrade.DealName = trade.DealName;
            existingTrade.DealType = trade.DealType;
            existingTrade.SourceListId = trade.SourceListId;
            existingTrade.Side = trade.Side;

            await _tradeRepository.UpdateAsync(existingTrade);
            return existingTrade;
        }

        public async Task<bool> DeleteTradeAsync(int id)
        {
            var trade = await _tradeRepository.GetByIdAsync(id);
            if (trade == null) return false;

            await _tradeRepository.DeleteAsync(trade);
            return true;
        }
    }
}
