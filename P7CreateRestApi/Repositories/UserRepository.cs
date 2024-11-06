using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;
using P7CreateRestApi.Repositories.Interface;
using P7CreateRestApi.Repositories;

namespace Dot.Net.WebApi.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(LocalDbContext dbContext) : base(dbContext)
        {
        }

        public User FindByUserName(string userName)
        {
            return _context.Users.FirstOrDefault(user => user.UserName == userName);
        }

        public async Task<List<User>> FindAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}