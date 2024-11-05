using Dot.Net.WebApi.Controllers.Domain;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.RatingDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;
        private readonly ILogger<RatingController> _logger;

        public RatingController(IRatingService ratingService, ILogger<RatingController> logger)
        {
            _ratingService = ratingService;
            _logger = logger;
        }

        // GET: api/rating/list
        [HttpGet("list")]
        public async Task<IActionResult> GetAllRatings()
        {
            _logger.LogInformation("Received request to GET all ratings");
            var ratings = await _ratingService.GetAllRatingsAsync();

            if (!ratings.Any())
            {
                _logger.LogWarning("No ratings found");
            }

            var ratingDtos = ratings.Select(r => new RatingDTO
            {
                Id = r.Id,
                MoodysRating = r.MoodysRating,
                SandPRating = r.SandPRating,
                FitchRating = r.FitchRating,
                OrderNumber = r.OrderNumber
            }).ToList();

            return Ok(ratingDtos);
        }

        // POST: api/rating/add
        [HttpPost("add")]
        public async Task<IActionResult> AddRating([FromBody] RatingCreateDTO ratingCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new rating");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for AddRating request");
                return BadRequest(ModelState);
            }

            var rating = new Rating
            {
                MoodysRating = ratingCreateDto.MoodysRating,
                SandPRating = ratingCreateDto.SandPRating,
                FitchRating = ratingCreateDto.FitchRating,
                OrderNumber = ratingCreateDto.OrderNumber
            };

            var createdRating = await _ratingService.CreateRatingAsync(rating);

            _logger.LogInformation("Created rating with ID {RatingId}", createdRating.Id);

            return CreatedAtAction(nameof(GetAllRatings), new { id = createdRating.Id }, createdRating);
        }

        // PUT: api/rating/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingUpdateDTO ratingUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE rating with ID {RatingId}", id);

            var existingRating = await _ratingService.GetRatingByIdAsync(id);
            if (existingRating == null)
            {
                _logger.LogWarning("Rating with ID {RatingId} not found for update", id);
                return NotFound();
            }

            existingRating.MoodysRating = ratingUpdateDto.MoodysRating;
            existingRating.SandPRating = ratingUpdateDto.SandPRating;
            existingRating.FitchRating = ratingUpdateDto.FitchRating;
            existingRating.OrderNumber = ratingUpdateDto.OrderNumber;

            var updatedRating = await _ratingService.UpdateRatingAsync(id, existingRating);

            _logger.LogInformation("Updated rating with ID {RatingId}", id);

            return Ok(updatedRating);
        }

        // DELETE: api/rating/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            _logger.LogInformation("Received request to DELETE rating with ID {RatingId}", id);

            var rating = await _ratingService.GetRatingByIdAsync(id);
            if (rating == null)
            {
                _logger.LogWarning("Rating with ID {RatingId} not found for deletion", id);
                return NotFound();
            }

            await _ratingService.DeleteRatingAsync(id);
            _logger.LogInformation("Deleted rating with ID {RatingId}", id);

            return NoContent();
        }
    }
}