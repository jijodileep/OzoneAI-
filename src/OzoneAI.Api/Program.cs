using Microsoft.EntityFrameworkCore;
using OzoneAI.Infrastructure;
using OzoneAI.Infrastructure.Persistence.Catalog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CatalogDbContext>("catalog-db");
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new
{
    service = "OzoneAI.Api",
    status = "ok"
}));

app.MapGet("/catalog/companies/count", async (CatalogDbContext db, CancellationToken ct) =>
{
    var count = await db.Companies.CountAsync(ct);
    return Results.Ok(new { companies = count });
});

app.Run();

public partial class Program;
