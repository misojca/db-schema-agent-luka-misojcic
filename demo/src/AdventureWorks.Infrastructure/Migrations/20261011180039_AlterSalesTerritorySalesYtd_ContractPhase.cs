using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterSalesTerritorySalesYtd_ContractPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase 2 — Contract: Finalize salesytd_new, remove old salesytd.
            // After Phase 1 data copy, salesytd_new contains all data from salesytd.
            // We drop the old column and rename salesytd_new → salesytd, then make it NOT NULL.
            //
            // The old salesytd column has multiple dependent objects:
            //   - CHECK constraint CK_SalesTerritory_SalesYTD
            //   - NOT NULL constraint salesterritory_salesytd_not_null
            //   - View sa.st (directly selects salesytd)
            // PostgreSQL blocks DROP COLUMN while these exist. We use raw SQL to
            // drop the column with CASCADE, which removes dependent constraints/views.
            // We then recreate the CHECK constraint and the view after renaming.

            // Drop old salesytd column with CASCADE (removes dependent constraints + views)
            migrationBuilder.Sql(@"
                ALTER TABLE sales.salesterritory
                DROP COLUMN salesytd CASCADE;
            ");

            // Rename salesytd_new → salesytd
            migrationBuilder.RenameColumn(
                name: "salesytd_new",
                schema: "sales",
                table: "salesterritory",
                newName: "salesytd");

            // Make salesytd NOT NULL with DEFAULT 0
            migrationBuilder.AlterColumn<decimal>(
                name: "salesytd",
                schema: "sales",
                table: "salesterritory",
                type: "numeric(19,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)",
                oldNullable: true);

            // Re-add the CHECK constraint on the new salesytd column
            migrationBuilder.Sql(@"
                ALTER TABLE sales.salesterritory
                ADD CONSTRAINT ""CK_SalesTerritory_SalesYTD""
                CHECK (salesytd >= 0.00);
            ");

            // Recreate the sa.st view that was dropped by CASCADE
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW sa.st AS
                SELECT territoryid AS id,
                       territoryid,
                       name,
                       countryregioncode,
                       ""group"",
                       salesytd,
                       saleslastyear,
                       costytd,
                       costlastyear,
                       rowguid,
                       modifieddate
                FROM sales.salesterritory;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the sa.st view
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS sa.st;");

            // Drop the CHECK constraint on the new salesytd column
            migrationBuilder.Sql(@"
                ALTER TABLE sales.salesterritory
                DROP CONSTRAINT IF EXISTS ""CK_SalesTerritory_SalesYTD"";
            ");

            // Restore old unconstrained numeric column (data precision beyond 4 dp is lost on rollback)
            migrationBuilder.AlterColumn<decimal>(
                name: "salesytd",
                schema: "sales",
                table: "salesterritory",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(19,4)");

            // Re-add the CHECK constraint on the old-style column
            migrationBuilder.Sql(@"
                ALTER TABLE sales.salesterritory
                ADD CONSTRAINT ""CK_SalesTerritory_SalesYTD""
                CHECK (salesytd >= 0.00);
            ");

            // Recreate the sa.st view
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW sa.st AS
                SELECT territoryid AS id,
                       territoryid,
                       name,
                       countryregioncode,
                       ""group"",
                       salesytd,
                       saleslastyear,
                       costytd,
                       costlastyear,
                       rowguid,
                       modifieddate
                FROM sales.salesterritory;
            ");

            // Re-add salesytd_new as nullable for symmetry with Phase 1
            migrationBuilder.AddColumn<decimal>(
                name: "salesytd_new",
                schema: "sales",
                table: "salesterritory",
                type: "numeric(19,4)",
                nullable: true);

            // Copy data back (precision beyond 4 decimal places is lost)
            migrationBuilder.Sql(@"
                UPDATE sales.salesterritory
                SET salesytd_new = CAST(salesytd AS numeric(19,4))
                WHERE salesytd IS NOT NULL;
            ");
        }
    }
}
