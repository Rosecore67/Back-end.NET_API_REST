using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.RuleNameDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RuleNameController : ControllerBase
    {
        private readonly IRuleNameService _ruleNameService;
        private readonly ILogger<RuleNameController> _logger;

        public RuleNameController(IRuleNameService ruleNameService, ILogger<RuleNameController> logger)
        {
            _ruleNameService = ruleNameService;
            _logger = logger;
        }

        // GET: api/rulename/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAllRuleNames()
        {
            _logger.LogInformation("Received request to GET all rule names");
            var ruleNames = await _ruleNameService.GetAllRuleNamesAsync();

            if (!ruleNames.Any())
            {
                _logger.LogWarning("No rule names found");
            }

            var ruleNameDtos = ruleNames.Select(r => new RuleNameDTO
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Json = r.Json,
                Template = r.Template,
                SqlStr = r.SqlStr,
                SqlPart = r.SqlPart
            }).ToList();

            return Ok(ruleNameDtos);
        }

        // POST: api/rulename/add
        [HttpPost("add")]
        public async Task<IActionResult> AddRuleName([FromBody] RuleNameCreateDTO ruleNameCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new rule name");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddRuleName request");
                return BadRequest(ModelState);
            }

            var ruleName = new RuleName
            {
                Name = ruleNameCreateDto.Name,
                Description = ruleNameCreateDto.Description,
                Json = ruleNameCreateDto.Json,
                Template = ruleNameCreateDto.Template,
                SqlStr = ruleNameCreateDto.SqlStr,
                SqlPart = ruleNameCreateDto.SqlPart
            };

            var createdRuleName = await _ruleNameService.CreateRuleNameAsync(ruleName);

            _logger.LogInformation("Created rule name with ID {RuleNameId}", createdRuleName.Id);

            return CreatedAtAction(nameof(GetAllRuleNames), new { id = createdRuleName.Id }, createdRuleName);
        }

        // PUT: api/rulename/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRuleName(int id, [FromBody] RuleNameUpdateDTO ruleNameUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE rule name with ID {RuleNameId}", id);

            var existingRuleName = await _ruleNameService.GetRuleNameByIdAsync(id);
            if (existingRuleName == null)
            {
                _logger.LogWarning("Rule name with ID {RuleNameId} not found for update", id);
                return NotFound();
            }

            existingRuleName.Name = ruleNameUpdateDto.Name;
            existingRuleName.Description = ruleNameUpdateDto.Description;
            existingRuleName.Json = ruleNameUpdateDto.Json;
            existingRuleName.Template = ruleNameUpdateDto.Template;
            existingRuleName.SqlStr = ruleNameUpdateDto.SqlStr;
            existingRuleName.SqlPart = ruleNameUpdateDto.SqlPart;

            var updatedRuleName = await _ruleNameService.UpdateRuleNameAsync(id, existingRuleName);

            _logger.LogInformation("Updated rule name with ID {RuleNameId}", id);

            return Ok(updatedRuleName);
        }

        // DELETE: api/rulename/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            _logger.LogInformation("Received request to DELETE rule name with ID {RuleNameId}", id);

            var ruleName = await _ruleNameService.GetRuleNameByIdAsync(id);
            if (ruleName == null)
            {
                _logger.LogWarning("Rule name with ID {RuleNameId} not found for deletion", id);
                return NotFound();
            }

            await _ruleNameService.DeleteRuleNameAsync(id);
            _logger.LogInformation("Deleted rule name with ID {RuleNameId}", id);

            return NoContent();
        }
    }
}