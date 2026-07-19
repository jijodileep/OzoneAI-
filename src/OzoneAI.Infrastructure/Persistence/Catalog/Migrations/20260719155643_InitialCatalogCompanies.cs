using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneAI.Infrastructure.Persistence.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class InitialCatalogCompanies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DatabaseName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DbHost = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    DbPort = table.Column<int>(type: "integer", nullable: false),
                    DbUsername = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DbPasswordProtected = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LegacyMigrationStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    TimeZoneId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastUsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ActiveUsers30d = table.Column<int>(type: "integer", nullable: false),
                    TotalUsersCached = table.Column<int>(type: "integer", nullable: false),
                    SchemaVersion = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_companies_CompanyKey",
                table: "companies",
                column: "CompanyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
