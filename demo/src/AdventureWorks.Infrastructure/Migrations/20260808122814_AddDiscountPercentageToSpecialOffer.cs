using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdventureWorks.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountPercentageToSpecialOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // sales.specialoffer is an existing table managed with ExcludeFromMigrations().
            // EF Core cannot emit AddColumn for excluded tables, so we use raw SQL.
            // Classification: Safe — nullable column with DEFAULT, no data loss risk.
            migrationBuilder.Sql(@"
                ALTER TABLE sales.specialoffer
                ADD COLUMN IF NOT EXISTS discount_percentage numeric(5,2) NULL DEFAULT 0.0;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reversal drops the column. Acceptable: column was just added, contains no
            // business-critical data at rollback time.
            migrationBuilder.Sql(@"
                ALTER TABLE sales.specialoffer
                DROP COLUMN IF EXISTS discount_percentage;
            ");
        }
    }
}
