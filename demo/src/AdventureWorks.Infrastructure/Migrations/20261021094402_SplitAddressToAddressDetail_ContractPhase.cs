using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitAddressToAddressDetail_ContractPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase 2 — Contract: remove spatiallocation from person.address.
            // Data was already migrated to person.addressdetail in Phase 1.
            //
            // Dependent view: pe.a selects spatiallocation from person.address.
            // Must drop it explicitly before dropping the column, then recreate
            // it without spatiallocation.

            // 1. Drop dependent view
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS pe.a;");

            // 2. Drop the old spatiallocation column from person.address
            migrationBuilder.Sql(@"ALTER TABLE person.address DROP COLUMN spatiallocation;");

            // 3. Recreate the view without spatiallocation
            migrationBuilder.Sql(@"
                CREATE VIEW pe.a AS
                SELECT addressid AS id,
                       addressid,
                       addressline1,
                       addressline2,
                       city,
                       stateprovinceid,
                       postalcode,
                       rowguid,
                       modifieddate
                FROM person.address;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // WARNING: On rollback, the old spatiallocation column is restored as nullable.
            // Data from addressdetail.spatiallocation is NOT copied back automatically.
            // This is an accepted data-loss risk on rollback per expand-contract pattern.

            // 1. Drop the recreated view
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS pe.a;");

            // 2. Restore spatiallocation column on person.address (nullable, no data)
            migrationBuilder.Sql(@"ALTER TABLE person.address ADD COLUMN spatiallocation bytea;");

            // 3. Copy data back from addressdetail to address (best-effort recovery)
            migrationBuilder.Sql(@"
                UPDATE person.address a
                SET spatiallocation = ad.spatiallocation
                FROM person.addressdetail ad
                WHERE a.addressid = ad.addressid;
            ");

            // 4. Recreate the original view with spatiallocation
            migrationBuilder.Sql(@"
                CREATE VIEW pe.a AS
                SELECT addressid AS id,
                       addressid,
                       addressline1,
                       addressline2,
                       city,
                       stateprovinceid,
                       postalcode,
                       spatiallocation,
                       rowguid,
                       modifieddate
                FROM person.address;
            ");
        }
    }
}
