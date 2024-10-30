using Dot.Net.WebApi.Controllers;
using Dot.Net.WebApi.Data;
using P7CreateRestApi.Repositories.Interface;

namespace P7CreateRestApi.Repositories
{
    public class RuleNameRepository : BaseRepository<RuleName>, IRuleNameRepository
    {
        public RuleNameRepository(LocalDbContext context) : base(context) { }
    }
}
