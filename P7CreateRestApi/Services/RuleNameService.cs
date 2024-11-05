using Dot.Net.WebApi.Controllers;
using P7CreateRestApi.Repositories.Interface;
using P7CreateRestApi.Services.Interface;

namespace P7CreateRestApi.Services
{
    public class RuleNameService : IRuleNameService
    {
        private readonly IRuleNameRepository _ruleNameRepository;

        public RuleNameService(IRuleNameRepository ruleNameRepository)
        {
            _ruleNameRepository = ruleNameRepository;
        }

        public async Task<IEnumerable<RuleName>> GetAllRuleNamesAsync()
        {
            return await _ruleNameRepository.GetAllAsync();
        }

        public async Task<RuleName> GetRuleNameByIdAsync(int id)
        {
            return await _ruleNameRepository.GetByIdAsync(id);
        }

        public async Task<RuleName> CreateRuleNameAsync(RuleName ruleName)
        {
            await _ruleNameRepository.AddAsync(ruleName);
            return ruleName;
        }

        public async Task<RuleName> UpdateRuleNameAsync(int id, RuleName ruleName)
        {
            var existingRule = await _ruleNameRepository.GetByIdAsync(id);
            if (existingRule == null) return null;

            existingRule.Name = ruleName.Name;
            existingRule.Description = ruleName.Description;
            existingRule.Json = ruleName.Json;
            existingRule.Template = ruleName.Template;
            existingRule.SqlStr = ruleName.SqlStr;
            existingRule.SqlPart = ruleName.SqlPart;

            await _ruleNameRepository.UpdateAsync(existingRule);
            return existingRule;
        }

        public async Task<bool> DeleteRuleNameAsync(int id)
        {
            var ruleName = await _ruleNameRepository.GetByIdAsync(id);
            if (ruleName == null) return false;

            await _ruleNameRepository.DeleteAsync(ruleName);
            return true;
        }
    }
}
