using Dot.Net.WebApi.Domain;
using P7CreateRestApi.Repositories.Interface;
using P7CreateRestApi.Services.Interface;

namespace P7CreateRestApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            // Logique supplémentaire, comme le hachage du mot de passe, pourrait être ajoutée ici.
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return null;

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;
            existingUser.Fullname = user.Fullname;
            existingUser.Role = user.Role;

            await _userRepository.UpdateAsync(existingUser);
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepository.DeleteAsync(user);
            return true;
        }

        public User FindByUserName(string userName)
        {
            return _userRepository.FindByUserName(userName);
        }
    }
}
