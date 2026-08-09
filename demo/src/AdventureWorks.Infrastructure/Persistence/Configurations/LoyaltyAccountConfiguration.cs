using AdventureWorks.Domain.Loyalty.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        // Table already exists in the database (created by an earlier migration).
        // ExcludeFromMigrations prevents EF from re-emitting CREATE TABLE.
        builder.ToTable("loyaltyaccount", "loyalty", t => t.ExcludeFromMigrations());

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

        // Index on FK column (cross-schema FK to sales.customer)
        builder.HasIndex(l => l.CustomerId)
            .HasDatabaseName("IX_loyaltyaccount_customerid")
            .IsUnique(); // one account per customer
    }
}
