using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OzoneAI.Application.Tenancy;
using OzoneAI.Domain.Catalog;
using OzoneAI.Infrastructure.Auth;

namespace OzoneAI.Infrastructure.Tenancy;

/// <summary>
/// After JWT auth, resolves the tenant DB connection from company_id claim.
/// GET/HEAD use Read (falls back to Write); other methods use Write.
/// </summary>
public sealed class TenantResolutionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        ITenantContext tenantContext,
        ITenantConnectionFactory connectionFactory)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true
            && user.FindFirstValue(JwtTokenService.AuthScopeClaim) == JwtTokenService.TenantScope)
        {
            var companyIdRaw = user.FindFirstValue(JwtTokenService.CompanyIdClaim);
            var companyKey = user.FindFirstValue(JwtTokenService.CompanyKeyClaim);
            if (Guid.TryParse(companyIdRaw, out var companyId) && !string.IsNullOrWhiteSpace(companyKey))
            {
                var preferRead = HttpMethods.IsGet(context.Request.Method)
                    || HttpMethods.IsHead(context.Request.Method);
                var role = preferRead ? TenantDbCredentialRole.Read : TenantDbCredentialRole.Write;

                var cs = await connectionFactory.GetConnectionStringAsync(
                    companyId,
                    role,
                    context.RequestAborted);

                Guid? impersonatedBy = null;
                var impRaw = user.FindFirstValue(JwtTokenService.ImpersonatedByClaim);
                if (Guid.TryParse(impRaw, out var impId))
                {
                    impersonatedBy = impId;
                }

                Guid? userId = null;
                var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (Guid.TryParse(sub, out var uid))
                {
                    userId = uid;
                }

                tenantContext.Set(
                    companyId,
                    companyKey,
                    cs,
                    role,
                    userId,
                    user.Identity?.Name,
                    user.FindFirstValue(ClaimTypes.Role),
                    impersonatedBy);
            }
        }

        await next(context);
    }
}
