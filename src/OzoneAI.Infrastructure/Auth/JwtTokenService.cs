using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OzoneAI.Application.Auth;

namespace OzoneAI.Infrastructure.Auth;

public sealed class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public const string AuthScopeClaim = "auth_scope";
    public const string PlatformScope = "platform";
    public const string TenantScope = "tenant";
    public const string SuperAdminRole = "SuperAdmin";
    public const string CompanyIdClaim = "company_id";
    public const string CompanyKeyClaim = "company_key";
    public const string ImpersonatedByClaim = "impersonated_by";

    public string IssueToken(IEnumerable<Claim> claims, TimeSpan? lifetime = null)
    {
        var key = configuration["Jwt:SigningKey"]
            ?? throw new InvalidOperationException("Jwt:SigningKey is not configured.");
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromHours(8));
        var jwt = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"] ?? "OzoneAI",
            audience: configuration["Jwt:Audience"] ?? "OzoneAI",
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
