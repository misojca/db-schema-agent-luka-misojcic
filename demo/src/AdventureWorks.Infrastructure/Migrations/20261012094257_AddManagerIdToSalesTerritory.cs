using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerIdToSalesTerritory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Issue #44: Add nullable manager_id column to sales.salesterritory
            // referencing humanresources.employee(businessentityid).
            // Safe change: nullable column, no data loss risk.

            migrationBuilder.AddColumn<int>(
                name: "manager_id",
                schema: "sales",
                table: "salesterritory",
                type: "integer",
                nullable: true);

            // FK to humanresources.employee(businessentityid)
            // Cross-schema FK: sales.salesterritory → humanresources.employee
            migrationBuilder.AddForeignKey(
                name: "FK_SalesTerritory_Employee_ManagerId",
                schema: "sales",
                table: "salesterritory",
                column: "manager_id",
                principalSchema: "humanresources",
                principalTable: "employee",
                principalColumn: "businessentityid",
                onDelete: ReferentialAction.Restrict);

            // Index on FK column per conventions
            migrationBuilder.CreateIndex(
                name: "IX_salesterritory_manager_id",
                schema: "sales",
                table: "salesterritory",
                column: "manager_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_salesterritory_manager_id",
                schema: "sales",
                table: "salesterritory");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesTerritory_Employee_ManagerId",
                schema: "sales",
                table: "salesterritory");

            migrationBuilder.DropColumn(
                name: "manager_id",
                schema: "sales",
                table: "salesterritory");
        }
    }
}
