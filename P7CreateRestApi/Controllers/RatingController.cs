using Dot.Net.WebApi.Controllers.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P7CreateRestApi.Models.DTOs.RatingDTOs;
using P7CreateRestApi.Services.Interface;

namespace Dot.Net.WebApi.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "Admin,User")]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllRatings()
        {
            _logger.LogInformation("Received request to GET all ratings");

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving ratings");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // POST: api/rating/add
        [Authorize(Roles = "Admin,User")]
        [HttpPost("add")]
        public async Task<IActionResult> AddRating([FromBody] RatingCreateDTO ratingCreateDto)
        {
            _logger.LogInformation("Received request to CREATE a new rating");

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new rating");
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // PUT: api/rating/update/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingUpdateDTO ratingUpdateDto)
        {
            _logger.LogInformation("Received request to UPDATE rating with ID {RatingId}", id);

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the rating with ID {RatingId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }

        // DELETE: api/rating/{id}
        [Authorize(Roles = "Admin,User")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            _logger.LogInformation("Received request to DELETE rating with ID {RatingId}", id);

            try
            {
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the rating with ID {RatingId}", id);
                return StatusCode(500, "An internal server error occurred. Please try again later.");
            }
        }
    }
}