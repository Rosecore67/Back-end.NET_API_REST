using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Repositories.Interface;

namespace P7CreateRestApi.Repositories
{
    public class TradeRepository : BaseRepository<Trade>, ITradeRepository
    {
        public TradeRepository(LocalDbContext context) : base(context) { }
    }
}
