using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterEmployee_AddPayColumns_ContractPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase 2 — Contract: make pay denormalisation columns NOT NULL.
            //
            // Prerequisite: Phase 1 (Expand) must have been applied and data migration
            // from employeepayhistory completed. All 290 employees have pay history
            // records, so all rows will have non-NULL values after Phase 1.
            //
            // employeepayhistory is NOT dropped — it retains historical pay data.
            //
            // Issue #7 — Scenario 6: Merge employeepayhistory into employee.

            migrationBuilder.AlterColumn<decimal>(
                name: "currentpayrate",
                schema: "humanresources",
                table: "employee",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<short>(
                name: "currentpayfrequency",
                schema: "humanresources",
                table: "employee",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "paychangedate",
                schema: "humanresources",
                table: "employee",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // WARNING: On rollback, columns revert to nullable.
            // employeepayhistory retains historical data so no data is lost.

            migrationBuilder.AlterColumn<decimal>(
                name: "currentpayrate",
                schema: "humanresources",
                table: "employee",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: false);

            migrationBuilder.AlterColumn<short>(
                name: "currentpayfrequency",
                schema: "humanresources",
                table: "employee",
                type: "smallint",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "paychangedate",
                schema: "humanresources",
                table: "employee",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: false);
        }
    }
}
