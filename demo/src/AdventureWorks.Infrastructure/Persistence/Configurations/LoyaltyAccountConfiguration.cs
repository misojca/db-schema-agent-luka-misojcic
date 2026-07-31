using AdventureWorks.Domain.Loyalty.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.ToTable("loyaltyaccount", "loyalty");
        builder.HasKey(l => l.LoyaltyAccountId).HasName("PK_Loyalty_LoyaltyAccount");
        builder.Property(l => l.LoyaltyAccountId).HasColumnName("loyaltyaccountid").ValueGeneratedOnAdd();
        builder.Property(l => l.CustomerId).HasColumnName("customerid").IsRequired();
        builder.Property(l => l.PointsBalance).HasColumnName("points_balance").HasColumnType("integer").IsRequired().HasDefaultValue(0);
        builder.Property(l => l.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone").IsRequired().HasDefaultValueSql("now()");
        builder.Property(l => l.ModifiedAt).HasColumnName("modified_at").HasColumnType("timestamp without time zone").IsRequired().HasDefaultValueSql("now()");
        builder.HasIndex(l => l.CustomerId).HasDatabaseName("IX_loyaltyaccount_customerid").IsUnique();
    }
}
