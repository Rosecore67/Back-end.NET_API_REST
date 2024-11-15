using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(string id);
        Task<User> FindByUserNameAsync(string userName);
        Task<User> CreateUserAsync(User user, string password);
        Task<User> UpdateUserAsync(string id, User user);
        Task<bool> DeleteUserAsync(string id);
        Task<User> AuthenticateAsync(string username, string password);
    }
}
