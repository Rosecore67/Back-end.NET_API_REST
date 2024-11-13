using Dot.Net.WebApi.Controllers;

namespace P7CreateRestApi.Services.Interface
{
    public interface IRuleNameService
    {
        Task<IEnumerable<RuleName>> GetAllRuleNamesAsync();
        Task<RuleName> GetRuleNameByIdAsync(int id);
        Task<RuleName> CreateRuleNameAsync(RuleName ruleName);
        Task<RuleName> UpdateRuleNameAsync(int id, RuleName ruleName);
        Task<bool> DeleteRuleNameAsync(int id);
    }
}
