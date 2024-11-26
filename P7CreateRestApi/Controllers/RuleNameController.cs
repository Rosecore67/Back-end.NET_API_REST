using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.RuleNameDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "Admin,User")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllRuleNames()
        {
            _logger.LogInformation("Received request to GET all rule names");

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all rule names");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // POST: api/rulename/add
        [Authorize(Roles = "Admin,User")]
        [HttpPost("add")]
        public async Task<IActionResult> AddRuleName([FromBody] RuleNameCreateDTO ruleNameCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new rule name");

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new rule name");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // PUT: api/rulename/update/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRuleName(int id, [FromBody] RuleNameUpdateDTO ruleNameUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE rule name with ID {RuleNameId}", id);

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the rule name with ID {RuleNameId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // DELETE: api/rulename/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRuleName(int id)
        {
            _logger.LogInformation("Received request to DELETE rule name with ID {RuleNameId}", id);

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the rule name with ID {RuleNameId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}