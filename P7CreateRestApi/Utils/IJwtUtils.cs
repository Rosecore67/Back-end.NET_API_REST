using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Utils
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(User user);
    }
}
