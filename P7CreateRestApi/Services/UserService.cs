using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;
using P7CreateRestApi.Repositories.Interface;
using P7CreateRestApi.Services.Interface;

namespace P7CreateRestApi.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return _userManager.Users.ToList();
        }

        public async Task<User> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded ? user : null;
        }

        public async Task<User> UpdateUserAsync(string id, User user)
        {
            var existingUser = await _userManager.FindByIdAsync(id);
            if (existingUser == null) return null;

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.Fullname = user.Fullname;
            existingUser.Role = user.Role;

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded ? existingUser : null;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<User> FindByUserNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }
    }
}
