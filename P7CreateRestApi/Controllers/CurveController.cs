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

        public CurveController(ICurvePointService curvePointService)
        {
            _curvePointService = curvePointService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetAllCurvePoints()
        {
            var curvePoints = await _curvePointService.GetAllCurvePointsAsync();
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

        [HttpPost("add")]
        public async Task<IActionResult> AddCurvePoint([FromBody] CurvePointCreateDTO curvePointCreateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var curvePoint = new CurvePoint
            {
                CurveId = curvePointCreateDto.CurveId,
                AsOfDate = curvePointCreateDto.AsOfDate,
                Term = curvePointCreateDto.Term,
                CurvePointValue = curvePointCreateDto.CurvePointValue
            };

            var createdCurvePoint = await _curvePointService.CreateCurvePointAsync(curvePoint);
            return CreatedAtAction(nameof(GetAllCurvePoints), new { id = createdCurvePoint.Id }, createdCurvePoint);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCurvePoint(int id, [FromBody] CurvePointUpdateDTO curvePointUpdateDto)
        {
            var existingCurvePoint = await _curvePointService.GetCurvePointByIdAsync(id);
            if (existingCurvePoint == null) return NotFound();

            existingCurvePoint.CurveId = curvePointUpdateDto.CurveId;
            existingCurvePoint.AsOfDate = curvePointUpdateDto.AsOfDate;
            existingCurvePoint.Term = curvePointUpdateDto.Term;
            existingCurvePoint.CurvePointValue = curvePointUpdateDto.CurvePointValue;

            await _curvePointService.UpdateCurvePointAsync(id, existingCurvePoint);
            return Ok(existingCurvePoint);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurvePoint(int id)
        {
            var curvePoint = await _curvePointService.GetCurvePointByIdAsync(id);
            if (curvePoint == null) return NotFound();

            await _curvePointService.DeleteCurvePointAsync(id);
            return NoContent();
        }
    }
}