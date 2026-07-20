using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Auth;
using OzoneAI.Application.Platform;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.FinancialYears;
using OzoneAI.Infrastructure.Persistence.Catalog;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Auth;

public sealed class TenantAuthService(
    CatalogDbContext catalog,
    ITenantLifecycleService lifecycle,
    ITenantConnectionFactory connectionFactory,
    IJwtTokenService jwt,
    IPasswordHasher<TenantUser> passwordHasher) : ITenantAuthService
{
    private static readonly TimeSpan DefaultLifetime = TimeSpan.FromHours(8);

    public async Task<TenantLoginResult?> LoginAsync(
        string companyKey,
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(companyKey)
            || string.IsNullOrWhiteSpace(username)
            || string.IsNullOrEmpty(password))
        {
            return null;
        }

        var access = await lifecycle.CheckLoginAccessAsync(companyKey, cancellationToken);
        if (!access.Allowed || access.CompanyId is null)
        {
            return null;
        }

        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == access.CompanyId.Value, cancellationToken);
        if (company is null)
        {
            return null;
        }

        var cs = await connectionFactory.GetConnectionStringAsync(
            company.Id,
            TenantDbCredentialRole.Write,
            cancellationToken);

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(cs)
            .Options;

        await using var tenantDb = new TenantDbContext(options, new FinancialYearContext());

        var normalizedUser = username.Trim();
        var user = await tenantDb.Users
            .FirstOrDefaultAsync(x => x.Username == normalizedUser, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var verify = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verify == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(user, password);
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await tenantDb.SaveChangesAsync(cancellationToken);

        var tracked = await catalog.Companies.FirstOrDefaultAsync(x => x.Id == company.Id, cancellationToken);
        if (tracked is not null)
        {
            tracked.LastUsedAt = DateTimeOffset.UtcNow;
            await catalog.SaveChangesAsync(cancellationToken);
        }

        return IssueResult(user, company, DefaultLifetime, impersonatedBy: null);
    }

    public async Task<TenantLoginResult> IssueTokenForUserAsync(
        Guid companyId,
        Guid userId,
        TimeSpan lifetime,
        Guid? impersonatedByPlatformUserId = null,
        CancellationToken cancellationToken = default)
    {
        var company = await catalog.Companies.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == companyId, cancellationToken)
            ?? throw new InvalidOperationException("Company not found.");

        var access = await lifecycle.CheckLoginAccessAsync(company.CompanyKey, cancellationToken);
        if (!access.Allowed)
        {
            throw new InvalidOperationException(access.Reason ?? "Company login is not allowed.");
        }

        var cs = await connectionFactory.GetConnectionStringAsync(
            companyId,
            TenantDbCredentialRole.Write,
            cancellationToken);

        var options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseNpgsql(cs)
            .Options;

        await using var tenantDb = new TenantDbContext(options, new FinancialYearContext());
        var user = await tenantDb.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId && x.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("Tenant user not found or inactive.");

        return IssueResult(user, company, lifetime, impersonatedByPlatformUserId);
    }

    private TenantLoginResult IssueResult(
        TenantUser user,
        Company company,
        TimeSpan lifetime,
        Guid? impersonatedBy)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role),
            new(JwtTokenService.AuthScopeClaim, JwtTokenService.TenantScope),
            new(JwtTokenService.CompanyIdClaim, company.Id.ToString()),
            new(JwtTokenService.CompanyKeyClaim, company.CompanyKey)
        };

        if (impersonatedBy is not null)
        {
            claims.Add(new Claim(JwtTokenService.ImpersonatedByClaim, impersonatedBy.Value.ToString()));
        }

        var token = jwt.IssueToken(claims, lifetime);
        return new TenantLoginResult(
            token,
            (int)lifetime.TotalSeconds,
            new TenantAuthUserDto(user.Id, user.Username, user.DisplayName, user.Role),
            new TenantAuthCompanyDto(company.Id, company.CompanyKey, company.Name),
            impersonatedBy);
    }
}
