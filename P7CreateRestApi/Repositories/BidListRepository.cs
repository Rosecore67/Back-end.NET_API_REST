using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Repositories.Interface;

namespace P7CreateRestApi.Repositories
{
    public class BidListRepository : BaseRepository<BidList>, IBidListRepository
    {
        public BidListRepository(LocalDbContext context) : base(context) { }
    }
}
