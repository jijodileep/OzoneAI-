using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneAI.Infrastructure.Persistence.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class SeparateTenantDbCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tenant_db_credentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    Host = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Port = table.Column<int>(type: "integer", nullable: false),
                    Username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    PasswordProtected = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RotatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenant_db_credentials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tenant_db_credentials_companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tenant_db_credentials_CompanyId_Role",
                table: "tenant_db_credentials",
                columns: new[] { "CompanyId", "Role" },
                unique: true,
                filter: "\"IsActive\" = TRUE");

            // Move credentials off companies into dedicated table (Write role).
            migrationBuilder.Sql("""
                INSERT INTO tenant_db_credentials
                    ("Id", "CompanyId", "Role", "Host", "Port", "Username", "PasswordProtected", "IsActive", "CreatedAt", "RotatedAt")
                SELECT
                    gen_random_uuid(),
                    "Id",
                    'Write',
                    "DbHost",
                    "DbPort",
                    "DbUsername",
                    "DbPasswordProtected",
                    TRUE,
                    COALESCE("CreatedAt", NOW()),
                    NULL
                FROM companies
                WHERE "DbHost" IS NOT NULL AND "DbHost" <> '';
                """);

            migrationBuilder.DropColumn(
                name: "DbHost",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "DbPasswordProtected",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "DbPort",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "DbUsername",
                table: "companies");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DbHost",
                table: "companies",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DbPasswordProtected",
                table: "companies",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DbPort",
                table: "companies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DbUsername",
                table: "companies",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE companies c
                SET
                    "DbHost" = cred."Host",
                    "DbPort" = cred."Port",
                    "DbUsername" = cred."Username",
                    "DbPasswordProtected" = cred."PasswordProtected"
                FROM tenant_db_credentials cred
                WHERE cred."CompanyId" = c."Id"
                  AND cred."Role" = 'Write'
                  AND cred."IsActive" = TRUE;
                """);

            migrationBuilder.DropTable(
                name: "tenant_db_credentials");
        }
    }
}
