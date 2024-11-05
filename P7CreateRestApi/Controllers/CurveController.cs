using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.CurveDTOs;
using P7CreateRestApi.Services.Interface;
using System.Security.Cryptography;

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
            _logger.LogInformation("Received request to GET all curves at {Time}", DateTime.UtcNow);
            var curvePoints = await _curvePointService.GetAllCurvePointsAsync();

            if(!curvePoints.Any())
            {
                _logger.LogWarning("No curves found at {Time}", DateTime.UtcNow);
            }
            else
            {
                _logger.LogInformation("Fetched {Count} curves at {Time}", curvePoints.Count(), DateTime.UtcNow);
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

        //POST: api/curve/add
        [HttpPost("add")]
        public async Task<IActionResult> AddCurvePoint([FromBody] CurvePointCreateDTO curvePointCreateDto)
        {
            _logger.LogInformation("Received request  to CREATE a new curve at {Time}", DateTime.UtcNow);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddCurvePoint request at {Time}", DateTime.UtcNow);
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

            _logger.LogInformation("Created curve with ID {CurveId} at {Time}", createdCurvePoint.Id, DateTime.UtcNow);

            return CreatedAtAction(nameof(GetAllCurvePoints), new { id = createdCurvePoint.Id }, createdCurvePoint);
        }

        //PUT: api/curve/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCurvePoint(int id, [FromBody] CurvePointUpdateDTO curvePointUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE bid with ID {CurveId} at {Time}", id, DateTime.UtcNow);

            var existingCurvePoint = await _curvePointService.GetCurvePointByIdAsync(id);

            if (existingCurvePoint == null)
            {
                _logger.LogWarning("Curve with ID {CurveId} not found for update at {Time}", id, DateTime.UtcNow);
                return NotFound();
            }

            existingCurvePoint.CurveId = curvePointUpdateDto.CurveId;
            existingCurvePoint.AsOfDate = curvePointUpdateDto.AsOfDate;
            existingCurvePoint.Term = curvePointUpdateDto.Term;
            existingCurvePoint.CurvePointValue = curvePointUpdateDto.CurvePointValue;

            await _curvePointService.UpdateCurvePointAsync(id, existingCurvePoint);

            _logger.LogInformation("Updated bid with ID {CurveId} at {Time}", id, DateTime.UtcNow);

            return Ok(existingCurvePoint);
        }

        //DELETE: api/curve/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            _logger.LogInformation("Received request to DELETE bid with ID {BidId} at {Time}", id, DateTime.UtcNow);

            var curvePoint = await _curvePointService.GetCurvePointByIdAsync(id);
            if (curvePoint == null)
            {
                _logger.LogWarning("Bid with ID {BidId} not found for deletion at {Time}", id, DateTime.UtcNow);
                return NotFound();
            }

            await _curvePointService.DeleteCurvePointAsync(id);
            _logger.LogInformation("Deleted curve with ID {CurveId} at {Time}", id, DateTime.UtcNow);


            return NoContent();
        }
    }
}