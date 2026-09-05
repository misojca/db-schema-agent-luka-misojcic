using AdventureWorks.Domain.Loyalty.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        // Table was originally created by migration 20260731125008_AddLoyaltyAccount.
        // ExcludeFromMigrations() has been removed so EF can emit ADD COLUMN for
        // new columns (e.g. tier_expiry_date added in issue #25).
        builder.ToTable("loyaltyaccount", "loyalty");

        builder.HasKey(l => l.LoyaltyAccountId)
            .HasName("PK_Loyalty_LoyaltyAccount");

        builder.Property(l => l.LoyaltyAccountId)
            .HasColumnName("loyaltyaccountid")
            .ValueGeneratedOnAdd();

        // Cross-schema FK: loyalty.loyaltyaccount.customerid → sales.customer.customerid
        builder.Property(l => l.CustomerId)
            .HasColumnName("customerid")
            .IsRequired();

        builder.Property(l => l.PointsBalance)
            .HasColumnName("points_balance")
            .HasColumnType("integer")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(l => l.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(l => l.ModifiedAt)
            .HasColumnName("modified_at")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Nullable column tracking when the current loyalty tier expires.
        // NULL = no active tier or tier does not expire.
        // Added in issue #25 (Scenario 8).
        builder.Property(l => l.TierExpiryDate)
            .HasColumnName("tier_expiry_date")
            .HasColumnType("timestamp without time zone")
            .IsRequired(false);

        // Index on FK column (cross-schema FK to sales.customer)
        builder.HasIndex(l => l.CustomerId)
            .HasDatabaseName("IX_loyaltyaccount_customerid")
            .IsUnique(); // one account per customer
    }
}
