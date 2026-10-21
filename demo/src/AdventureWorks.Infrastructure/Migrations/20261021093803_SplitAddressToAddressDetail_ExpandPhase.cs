using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SplitAddressToAddressDetail_ExpandPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Phase 1 — Expand: create person.addressdetail table alongside person.address.
            // spatiallocation remains on person.address during this phase.
            // Data is copied from person.address to person.addressdetail.

            migrationBuilder.CreateTable(
                name: "addressdetail",
                schema: "person",
                columns: table => new
                {
                    addressid = table.Column<int>(type: "integer", nullable: false),
                    spatiallocation = table.Column<byte[]>(type: "bytea", nullable: true),
                    modifieddate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person_AddressDetail", x => x.addressid);
                    table.ForeignKey(
                        name: "FK_AddressDetail_Address_AddressId",
                        column: x => x.addressid,
                        principalSchema: "person",
                        principalTable: "address",
                        principalColumn: "addressid",
                        onDelete: ReferentialAction.Cascade);
                });

            // Migrate existing spatiallocation data from person.address to person.addressdetail.
            // Insert one row per address, copying the spatiallocation value.
            migrationBuilder.Sql(@"
                INSERT INTO person.addressdetail (addressid, spatiallocation, modifieddate)
                SELECT addressid, spatiallocation, modifieddate
                FROM person.address
                WHERE spatiallocation IS NOT NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "addressdetail",
                schema: "person");
        }
    }
}
