using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterEmployee_AddPayColumns_ExpandPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase 1 — Expand: add nullable pay denormalisation columns to humanresources.employee.
            // Columns are nullable during the transition period.
            // Data is migrated from humanresources.employeepayhistory (latest record per employee).
            //
            // Issue #7 — Scenario 6: Merge employeepayhistory into employee.

            migrationBuilder.AddColumn<decimal>(
                name: "currentpayrate",
                schema: "humanresources",
                table: "employee",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "currentpayfrequency",
                schema: "humanresources",
                table: "employee",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "paychangedate",
                schema: "humanresources",
                table: "employee",
                type: "timestamp without time zone",
                nullable: true);

            // Migrate the most recent pay record per employee from employeepayhistory.
            // Uses a subquery to find the latest ratechangedate per businessentityid.
            migrationBuilder.Sql(@"
                UPDATE humanresources.employee e
                SET
                    currentpayrate = ph.rate,
                    currentpayfrequency = ph.payfrequency,
                    paychangedate = ph.ratechangedate
                FROM humanresources.employeepayhistory ph
                WHERE ph.businessentityid = e.businessentityid
                  AND ph.ratechangedate = (
                      SELECT MAX(inner_ph.ratechangedate)
                      FROM humanresources.employeepayhistory inner_ph
                      WHERE inner_ph.businessentityid = e.businessentityid
                  );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currentpayrate",
                schema: "humanresources",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "currentpayfrequency",
                schema: "humanresources",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "paychangedate",
                schema: "humanresources",
                table: "employee");
        }
    }
}
