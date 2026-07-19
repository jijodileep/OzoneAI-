using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OzoneAI.Domain.Tenant;
using OzoneAI.Infrastructure.Persistence.Tenant;

namespace OzoneAI.Infrastructure.Tenancy;

public sealed class TenantDataSeeder(IPasswordHasher<TenantUser> passwordHasher)
{
    public async Task SeedDefaultsAsync(
        TenantDbContext tenant,
        TenantSeedOptions options,
        CancellationToken cancellationToken = default)
    {
        if (!await tenant.CompanyProfiles.AnyAsync(cancellationToken))
        {
            tenant.CompanyProfiles.Add(new CompanyProfile
            {
                Id = Guid.NewGuid(),
                LegalName = options.LegalName,
                Address = options.Address,
                Phone = options.Phone,
                Email = options.Email,
                UpdatedAt = DateTimeOffset.UtcNow
            });
            tenant.CompanySettings.Add(new CompanySettings
            {
                Id = Guid.NewGuid(),
                TaxType = options.TaxType,
                CurrencyCode = options.CurrencyCode,
                CurrencySymbol = options.CurrencyCode == "INR" ? "₹" : options.CurrencyCode,
                UpdatedAt = DateTimeOffset.UtcNow
            });
            tenant.CompanyBranches.Add(new CompanyBranch
            {
                Id = Guid.NewGuid(),
                Name = "Main",
                Address = options.Address,
                IsMain = true,
                UpdatedAt = DateTimeOffset.UtcNow
            });
        }

        if (!await tenant.FinancialYears.AnyAsync(cancellationToken))
        {
            var start = new DateOnly(DateTime.UtcNow.Year, 4, 1);
            if (DateOnly.FromDateTime(DateTime.UtcNow) < start)
            {
                start = start.AddYears(-1);
            }

            tenant.FinancialYears.Add(new FinancialYear
            {
                Id = Guid.NewGuid(),
                Name = $"{start.Year}-{(start.Year + 1).ToString()[^2..]}",
                StartDate = start,
                EndDate = start.AddYears(1).AddDays(-1),
                Status = FinancialYearStatus.Open,
                IsDefault = true,
                CreatedAt = DateTimeOffset.UtcNow
            });
        }

        if (!string.IsNullOrWhiteSpace(options.AdminUsername)
            && !await tenant.Users.AnyAsync(x => x.Username == options.AdminUsername, cancellationToken))
        {
            var user = new TenantUser
            {
                Id = Guid.NewGuid(),
                Username = options.AdminUsername.Trim(),
                DisplayName = string.IsNullOrWhiteSpace(options.AdminDisplayName)
                    ? options.AdminUsername.Trim()
                    : options.AdminDisplayName.Trim(),
                Role = TenantRoles.Admin,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };
            user.PasswordHash = passwordHasher.HashPassword(user, options.AdminPassword);
            tenant.Users.Add(user);
        }

        await tenant.SaveChangesAsync(cancellationToken);
    }
}

public sealed record TenantSeedOptions(
    string LegalName,
    string Address,
    string Phone,
    string? Email,
    string TaxType,
    string CurrencyCode,
    string AdminUsername,
    string AdminPassword,
    string? AdminDisplayName);
