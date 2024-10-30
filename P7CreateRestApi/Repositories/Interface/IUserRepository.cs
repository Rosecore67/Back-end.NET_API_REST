using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Repositories.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        User FindByUserName(string userName);
        Task<List<User>> FindAllAsync();
    }
}
