using Dot.Net.WebApi.Controllers.Domain;

namespace P7CreateRestApi.Services.Interface
{
    public interface IRatingService
    {
        Task<IEnumerable<Rating>> GetAllRatingsAsync();
        Task<Rating> GetRatingByIdAsync(int id);
        Task<Rating> CreateRatingAsync(Rating rating);
        Task<Rating> UpdateRatingAsync(int id,Rating rating);
        Task<bool> DeleteRatingAsync(int id);
    }
}
