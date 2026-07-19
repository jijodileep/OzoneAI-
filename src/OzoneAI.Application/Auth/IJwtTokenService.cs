using System.Security.Claims;

namespace OzoneAI.Application.Auth;

public interface IJwtTokenService
{
    string IssueToken(IEnumerable<Claim> claims, TimeSpan? lifetime = null);
}
