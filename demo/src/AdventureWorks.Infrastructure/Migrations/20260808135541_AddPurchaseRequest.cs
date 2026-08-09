using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "purchasing");

            migrationBuilder.CreateTable(
                name: "purchaserequest",
                schema: "purchasing",
                columns: table => new
                {
                    purchaserequestid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    employeeid = table.Column<int>(type: "integer", nullable: false),
                    productid = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<short>(type: "smallint", nullable: false),
                    requestdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    status = table.Column<string>(type: "character varying(20)", nullable: false, defaultValue: "pending"),
                    notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchasing_PurchaseRequest", x => x.purchaserequestid);
                });

            // Cross-schema FK constraints added via raw SQL.
            // EF cannot auto-emit these because the referenced tables use ExcludeFromMigrations()
            // or belong to schemas not owned by this DbContext.
            // Named per convention: FK_<Table>_<RefTable>_<Column>
            migrationBuilder.Sql(@"
                ALTER TABLE purchasing.purchaserequest
                ADD CONSTRAINT ""FK_PurchaseRequest_Employee_EmployeeId""
                FOREIGN KEY (employeeid)
                REFERENCES humanresources.employee (businessentityid)
                ON DELETE RESTRICT;
            ");

            migrationBuilder.Sql(@"
                ALTER TABLE purchasing.purchaserequest
                ADD CONSTRAINT ""FK_PurchaseRequest_Product_ProductId""
                FOREIGN KEY (productid)
                REFERENCES production.product (productid)
                ON DELETE RESTRICT;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequest_employeeid",
                schema: "purchasing",
                table: "purchaserequest",
                column: "employeeid");

            migrationBuilder.CreateIndex(
                name: "IX_purchaserequest_productid",
                schema: "purchasing",
                table: "purchaserequest",
                column: "productid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchaserequest",
                schema: "purchasing");
        }
    }
}
