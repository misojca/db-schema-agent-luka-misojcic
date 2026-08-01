using AdventureWorks.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class CurrencyRateConfiguration : IEntityTypeConfiguration<CurrencyRate>
{
    public void Configure(EntityTypeBuilder<CurrencyRate> builder)
    {
        // Table pre-existed in DB — baseline registered via AddCurrencyRateBaseline.
        // ExcludeFromMigrations removed — EF now manages incremental DDL changes.
        builder.ToTable("currencyrate", "sales");

        builder.HasKey(c => c.CurrencyRateId)
            .HasName("PK_Sales_CurrencyRate");

        builder.Property(c => c.CurrencyRateId)
            .HasColumnName("currencyrateid")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.CurrencyRateDate)
            .HasColumnName("currencyratedate")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(c => c.FromCurrencyCode)
            .HasColumnName("fromcurrencycode")
            .HasColumnType("character")
            .IsRequired();

        builder.Property(c => c.ToCurrencyCode)
            .HasColumnName("tocurrencycode")
            .HasColumnType("character")
            .IsRequired();

        // Issue #4: AverageRate currently maps to the original averagerate column (numeric).
        // Phase 1 expand will add averagerate_new (numeric(19,4)) alongside this.
        // Phase 2 contract will remap AverageRate to averagerate_new and drop averagerate.
        builder.Property(c => c.AverageRate)
            .HasColumnName("averagerate")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(c => c.EndOfDayRate)
            .HasColumnName("endofdayrate")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(c => c.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}
