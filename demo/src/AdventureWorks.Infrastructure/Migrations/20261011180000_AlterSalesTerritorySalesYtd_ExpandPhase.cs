using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterSalesTerritorySalesYtd_ExpandPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase 1 — Expand: Add salesytd_new alongside existing salesytd.
            // salesytd is currently numeric (unconstrained). Adding salesytd_new as
            // numeric(19,4) nullable so we can safely copy data before the contract phase.

            migrationBuilder.AddColumn<decimal>(
                name: "salesytd_new",
                schema: "sales",
                table: "salesterritory",
                type: "numeric(19,4)",
                nullable: true);

            // Copy data from old column to new column
            migrationBuilder.Sql(@"
                UPDATE sales.salesterritory
                SET salesytd_new = CAST(salesytd AS numeric(19,4))
                WHERE salesytd IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "salesytd_new",
                schema: "sales",
                table: "salesterritory");
        }
    }
}
