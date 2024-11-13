using Dot.Net.WebApi.Controllers.Domain;
using P7CreateRestApi.Repositories.Interface;
using P7CreateRestApi.Services.Interface;
using System.Runtime.CompilerServices;

namespace P7CreateRestApi.Services
{
    public class RatingService : IRatingService
    {    
        private readonly IRatingRepository _ratingRepository;
        
        public RatingService(IRatingRepository ratingRepository) 
        {
            _ratingRepository = ratingRepository;
        }

        public async Task<IEnumerable<Rating>> GetAllRatingsAsync()
        {
            return await _ratingRepository.GetAllAsync();
        }

        public async Task<Rating> GetRatingByIdAsync(int id)
        {
            return await _ratingRepository.GetByIdAsync(id);
        }
        public async Task<Rating> CreateRatingAsync(Rating rating)
        {
            await _ratingRepository.AddAsync(rating);
            return rating;
        }
        public async Task<Rating> UpdateRatingAsync(int id, Rating rating)
        {
            var existingRating = await _ratingRepository.GetByIdAsync(id);
            if (existingRating == null) return null;

            existingRating.MoodysRating = rating.MoodysRating;
            existingRating.SandPRating = rating.SandPRating;
            existingRating.FitchRating = rating.FitchRating;
            existingRating.OrderNumber = rating.OrderNumber;

            await _ratingRepository.UpdateAsync(existingRating);
            return existingRating;
        }

        public async Task<bool> DeleteRatingAsync(int id)
        {
            var rating = await _ratingRepository.GetByIdAsync(id);
            if (rating == null) return false;

            await _ratingRepository.DeleteAsync(rating); 
            return true;
        }
    }
}
