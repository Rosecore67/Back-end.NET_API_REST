using Dot.Net.WebApi.Domain;
using Microsoft.Extensions.Configuration;
using P7CreateRestApi.Utils;

namespace P7CreateRestApi.Tests.Fakes
{
    public class FakeJwtUtils : JwtUtils
    {
        public FakeJwtUtils() : base(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            { "Jwt:Key", "TestKey123456789012345678901234567890" },
            { "Jwt:Issuer", "TestIssuer" },
            { "Jwt:Audience", "TestAudience" }
        }).Build())
        {
        }

        public override string GenerateJwtToken(User user)
        {
            return "fake-jwt-token";
        }
    }
}
