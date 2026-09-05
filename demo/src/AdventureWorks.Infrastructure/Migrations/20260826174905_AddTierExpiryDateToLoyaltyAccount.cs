using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTierExpiryDateToLoyaltyAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Scenario 8 — issue #25
            // Add nullable tier_expiry_date column to loyalty.loyaltyaccount.
            // Tracks when the customer's current loyalty tier expires.
            // NULL = no active tier or tier does not expire.
            migrationBuilder.AddColumn<DateTime>(
                name: "tier_expiry_date",
                schema: "loyalty",
                table: "loyaltyaccount",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tier_expiry_date",
                schema: "loyalty",
                table: "loyaltyaccount");
        }
    }
}
