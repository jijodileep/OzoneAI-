using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OzoneAI.Application.FinancialYears;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.FinancialYears;

public sealed class FinancialYearSwitchService(
    TenantDbContext db,
    IFinancialYearContext fyContext,
    IConfiguration configuration) : IFinancialYearSwitchService
{
    public async Task<FinancialYearSwitchResult> SwitchAsync(Guid financialYearId, CancellationToken cancellationToken = default)
    {
        var year = await db.FinancialYears.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == financialYearId, cancellationToken)
            ?? throw new InvalidOperationException("Financial year not found.");

        var readOnly = year.Status != FinancialYearStatus.Open;
        fyContext.Set(year.Id, readOnly);

        var token = IssueDevToken(year);
        return new FinancialYearSwitchResult(year.Id, year.Name, readOnly, token);
    }

    private string IssueDevToken(FinancialYear year)
    {
        var key = configuration["Jwt:SigningKey"] ?? "OzoneAI-dev-signing-key-change-me-32chars!";
        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("financial_year_id", year.Id.ToString()),
            new Claim("financial_year_name", year.Name),
            new Claim("financial_year_readonly", (year.Status != FinancialYearStatus.Open).ToString())
        };

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
