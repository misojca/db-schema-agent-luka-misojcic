using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "loyalty");

            migrationBuilder.CreateTable(
                name: "loyaltyaccount",
                schema: "loyalty",
                columns: table => new
                {
                    loyaltyaccountid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    customerid = table.Column<int>(type: "integer", nullable: false),
                    points_balance = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loyalty_LoyaltyAccount", x => x.loyaltyaccountid);
                    // Cross-schema FK: loyalty.loyaltyaccount.customerid → sales.customer.customerid
                    // Issue #5 — Scenario 4.
                    table.ForeignKey(
                        name: "FK_LoyaltyAccount_Customer_CustomerID",
                        column: x => x.customerid,
                        principalSchema: "sales",
                        principalTable: "customer",
                        principalColumn: "customerid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_loyaltyaccount_customerid",
                schema: "loyalty",
                table: "loyaltyaccount",
                column: "customerid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loyaltyaccount",
                schema: "loyalty");
        }
    }
}
