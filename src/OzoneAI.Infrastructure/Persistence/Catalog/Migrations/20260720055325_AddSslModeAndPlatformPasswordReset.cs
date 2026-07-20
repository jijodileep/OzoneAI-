using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneAI.Infrastructure.Persistence.Catalog.Migrations
{
    /// <inheritdoc />
    public partial class AddSslModeAndPlatformPasswordReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SslMode",
                table: "tenant_db_credentials",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "platform_users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "platform_password_reset_tokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PlatformUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_platform_password_reset_tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_platform_password_reset_tokens_platform_users_PlatformUserId",
                        column: x => x.PlatformUserId,
                        principalTable: "platform_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_platform_password_reset_tokens_PlatformUserId",
                table: "platform_password_reset_tokens",
                column: "PlatformUserId");

            migrationBuilder.CreateIndex(
                name: "IX_platform_password_reset_tokens_TokenHash",
                table: "platform_password_reset_tokens",
                column: "TokenHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "platform_password_reset_tokens");

            migrationBuilder.DropColumn(
                name: "SslMode",
                table: "tenant_db_credentials");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "platform_users");
        }
    }
}
