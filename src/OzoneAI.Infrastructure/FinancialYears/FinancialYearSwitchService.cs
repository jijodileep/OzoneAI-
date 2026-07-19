using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using OzoneAI.Application.Auth;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Auth;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.FinancialYears;

public sealed class FinancialYearSwitchService(
    TenantDbContext db,
    IFinancialYearContext fyContext,
    IJwtTokenService jwt) : IFinancialYearSwitchService
{
    public async Task<FinancialYearSwitchResult> SwitchAsync(Guid financialYearId, CancellationToken cancellationToken = default)
    {
        var year = await db.FinancialYears.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == financialYearId, cancellationToken)
            ?? throw new InvalidOperationException("Financial year not found.");

        var readOnly = year.Status != FinancialYearStatus.Open;
        fyContext.Set(year.Id, readOnly);

        var claims = new Claim[]
        {
            new("financial_year_id", year.Id.ToString()),
            new("financial_year_name", year.Name),
            new("financial_year_readonly", readOnly.ToString()),
            new(JwtTokenService.AuthScopeClaim, JwtTokenService.TenantScope)
        };

        var token = jwt.IssueToken(claims);
        return new FinancialYearSwitchResult(year.Id, year.Name, readOnly, token);
    }
}
