using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.CurveDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurveController : ControllerBase
    {
        private readonly ICurvePointService _curvePointService;
        private readonly ILogger<CurveController> _logger;

        public CurveController(ICurvePointService curvePointService, ILogger<CurveController> logger)
        {
            _curvePointService = curvePointService;
            _logger = logger;
        }

        // GET: api/curve/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAllCurvePoints()
        {
            _logger.LogInformation("Received request to GET all curves at {Time}", DateTime.Now);

            try
            {
                var curvePoints = await _curvePointService.GetAllCurvePointsAsync();

                if (!curvePoints.Any())
                {
                    _logger.LogWarning("No curves found at {Time}", DateTime.Now);
                }
                else
                {
                    _logger.LogInformation("Fetched {Count} curves at {Time}", curvePoints.Count(), DateTime.Now);
                }

                var curvePointDtos = curvePoints.Select(cp => new CurvePointDTO
                {
                    Id = cp.Id,
                    CurveId = cp.CurveId,
                    AsOfDate = cp.AsOfDate,
                    Term = cp.Term,
                    CurvePointValue = cp.CurvePointValue,
                    CreationDate = cp.CreationDate
                }).ToList();

                return Ok(curvePointDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all curves at {Time}", DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        //POST: api/curve/add
        [HttpPost("add")]
        public async Task<IActionResult> AddCurvePoint([FromBody] CurvePointCreateDTO curvePointCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new curve at {Time}", DateTime.Now);

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for AddCurvePoint request at {Time}", DateTime.Now);
                    return BadRequest(ModelState);
                }

                var curvePoint = new CurvePoint
                {
                    CurveId = curvePointCreateDto.CurveId,
                    AsOfDate = curvePointCreateDto.AsOfDate,
                    Term = curvePointCreateDto.Term,
                    CurvePointValue = curvePointCreateDto.CurvePointValue
                };

                var createdCurvePoint = await _curvePointService.CreateCurvePointAsync(curvePoint);

                _logger.LogInformation("Created curve with ID {CurveId} at {Time}", createdCurvePoint.Id, DateTime.Now);

                return CreatedAtAction(nameof(GetAllCurvePoints), new { id = createdCurvePoint.Id }, createdCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new curve at {Time}", DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        //PUT: api/curve/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCurvePoint(int id, [FromBody] CurvePointUpdateDTO curvePointUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE curve with ID {CurveId} at {Time}", id, DateTime.Now);

            try
            {
                var existingCurvePoint = await _curvePointService.GetCurvePointByIdAsync(id);

                if (existingCurvePoint == null)
                {
                    _logger.LogWarning("Curve with ID {CurveId} not found for update at {Time}", id, DateTime.Now);
                    return NotFound();
                }

                existingCurvePoint.CurveId = curvePointUpdateDto.CurveId;
                existingCurvePoint.AsOfDate = curvePointUpdateDto.AsOfDate;
                existingCurvePoint.Term = curvePointUpdateDto.Term;
                existingCurvePoint.CurvePointValue = curvePointUpdateDto.CurvePointValue;

                var updatedCurvePoint = await _curvePointService.UpdateCurvePointAsync(id, existingCurvePoint);

                _logger.LogInformation("Updated curve with ID {CurveId} at {Time}", id, DateTime.Now);

                return Ok(updatedCurvePoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the curve with ID {CurveId} at {Time}", id, DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        //DELETE: api/curve/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            _logger.LogInformation("Received request to DELETE curve with ID {CurveId} at {Time}", id, DateTime.Now);

            try
            {
                var curvePoint = await _curvePointService.GetCurvePointByIdAsync(id);
                if (curvePoint == null)
                {
                    _logger.LogWarning("Curve with ID {CurveId} not found for deletion at {Time}", id, DateTime.Now);
                    return NotFound();
                }

                await _curvePointService.DeleteCurvePointAsync(id);
                _logger.LogInformation("Deleted curve with ID {CurveId} at {Time}", id, DateTime.Now);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the curve with ID {CurveId} at {Time}", id, DateTime.Now);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}