using AdventureWorks.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for sales.specialoffer.
/// The table pre-exists in the database — all existing columns are mapped
/// with ExcludeFromMigrations() so EF Core will not attempt to recreate them.
/// Only discount_percentage is a new column managed by EF Core migrations.
/// </summary>
public class SpecialOfferConfiguration : IEntityTypeConfiguration<SpecialOffer>
{
    public void Configure(EntityTypeBuilder<SpecialOffer> builder)
    {
        // Table exists — exclude from migrations to prevent re-creation of the table.
        // ExcludeFromMigrations is set at table level; the new column is emitted separately.
        builder.ToTable("specialoffer", "sales", t => t.ExcludeFromMigrations());

        builder.HasKey(s => s.SpecialOfferId)
            .HasName("PK_Sales_SpecialOffer");

        builder.Property(s => s.SpecialOfferId)
            .HasColumnName("specialofferid")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Description)
            .HasColumnName("description")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(s => s.DiscountPct)
            .HasColumnName("discountpct")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(s => s.Type)
            .HasColumnName("type")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(s => s.Category)
            .HasColumnName("category")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(s => s.StartDate)
            .HasColumnName("startdate")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.EndDate)
            .HasColumnName("enddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(s => s.MinQty)
            .HasColumnName("minqty")
            .HasColumnType("integer")
            .HasDefaultValue(0)
            .IsRequired();

        builder.Property(s => s.MaxQty)
            .HasColumnName("maxqty")
            .HasColumnType("integer");

        builder.Property(s => s.Rowguid)
            .HasColumnName("rowguid")
            .HasColumnType("uuid")
            .HasDefaultValueSql("uuid_generate_v1()")
            .IsRequired();

        builder.Property(s => s.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("now()")
            .IsRequired();

        // New column — managed by EF Core migration AddDiscountPercentageToSpecialOffer.
        builder.Property(s => s.DiscountPercentage)
            .HasColumnName("discount_percentage")
            .HasColumnType("numeric(5,2)")
            .HasDefaultValue(0.0m);
    }
}
