using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OzoneAI.Infrastructure.Persistence.Tenant.Migrations
{
    /// <inheritdoc />
    public partial class InitialTenantFinancialYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "company_branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TaxNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsMain = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "company_profile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Address = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    AddressLocal = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Phone = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    TaxNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Fssai = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    LogoObjectKey = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    BankName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BankAccountNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    BankIfsc = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_profile", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "company_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaxType = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    CurrencySymbol = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    StateCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    MultiGodown = table.Column<bool>(type: "boolean", nullable: false),
                    BatchEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    FeatureFlagsJson = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_company_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "financial_years",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    ClosedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ClosedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_financial_years", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sample_sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartyId = table.Column<Guid>(type: "uuid", nullable: true),
                    GrandTotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    FinancialYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DocumentNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sample_sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ledger_opening_balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    LedgerId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    DrCr = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    OpeningBalancePaid = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_opening_balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ledger_opening_balances_financial_years_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "financial_years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_opening_balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialYearId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    GodownId = table.Column<Guid>(type: "uuid", nullable: false),
                    BatchNo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OpeningQty = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    OpeningRate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    OpeningValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_opening_balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stock_opening_balances_financial_years_FinancialYearId",
                        column: x => x.FinancialYearId,
                        principalTable: "financial_years",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_financial_years_Name",
                table: "financial_years",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_financial_years_StartDate_EndDate",
                table: "financial_years",
                columns: new[] { "StartDate", "EndDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ledger_opening_balances_FinancialYearId_LedgerId",
                table: "ledger_opening_balances",
                columns: new[] { "FinancialYearId", "LedgerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sample_sales_FinancialYearId_DocumentDate",
                table: "sample_sales",
                columns: new[] { "FinancialYearId", "DocumentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_sample_sales_FinancialYearId_DocumentNo",
                table: "sample_sales",
                columns: new[] { "FinancialYearId", "DocumentNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_opening_balances_FinancialYearId_ItemId_GodownId_Batc~",
                table: "stock_opening_balances",
                columns: new[] { "FinancialYearId", "ItemId", "GodownId", "BatchNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "company_branches");

            migrationBuilder.DropTable(
                name: "company_profile");

            migrationBuilder.DropTable(
                name: "company_settings");

            migrationBuilder.DropTable(
                name: "ledger_opening_balances");

            migrationBuilder.DropTable(
                name: "sample_sales");

            migrationBuilder.DropTable(
                name: "stock_opening_balances");

            migrationBuilder.DropTable(
                name: "financial_years");
        }
    }
}
