using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Repositories.Interface;

namespace P7CreateRestApi.Repositories
{
    public class CurvePointRepository : BaseRepository<CurvePoint> , ICurvePointRepository
    {
        public CurvePointRepository(LocalDbContext context) : base(context) { }
    }
}
