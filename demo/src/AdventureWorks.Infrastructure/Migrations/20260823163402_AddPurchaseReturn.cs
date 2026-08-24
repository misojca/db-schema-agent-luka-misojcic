using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPurchaseReturn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "purchasereturn",
                schema: "purchasing",
                columns: table => new
                {
                    purchasereturnid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    purchaseorderdetailid = table.Column<int>(type: "integer", nullable: false),
                    returndate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    quantity = table.Column<short>(type: "smallint", nullable: false),
                    reason = table.Column<string>(type: "character varying(200)", nullable: true),
                    modifieddate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchasing_PurchaseReturn", x => x.purchasereturnid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_purchasereturn_purchaseorderdetailid",
                schema: "purchasing",
                table: "purchasereturn",
                column: "purchaseorderdetailid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "purchasereturn",
                schema: "purchasing");
        }
    }
}
