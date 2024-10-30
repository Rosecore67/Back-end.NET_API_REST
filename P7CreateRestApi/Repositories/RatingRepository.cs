using Dot.Net.WebApi.Controllers.Domain;
using Dot.Net.WebApi.Data;
using P7CreateRestApi.Repositories.Interface;

namespace P7CreateRestApi.Repositories
{
    public class RatingRepository : BaseRepository<Rating> , IRatingRepository
    {
        public RatingRepository(LocalDbContext context) : base(context) { }
    }
}
