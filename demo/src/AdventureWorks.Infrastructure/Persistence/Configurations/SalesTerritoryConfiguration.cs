using AdventureWorks.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class SalesTerritoryConfiguration : IEntityTypeConfiguration<SalesTerritory>
{
    public void Configure(EntityTypeBuilder<SalesTerritory> builder)
    {
        // Table pre-existed in DB — excluded from migrations.
        // Issue #42: salesytd type change from numeric to numeric(19,4)
        // handled via expand-contract migrations (manually written).
        // Entity config reflects the FINAL desired state (numeric(19,4)).
        builder.ToTable("salesterritory", "sales", t => t.ExcludeFromMigrations());

        builder.HasKey(st => st.TerritoryId)
            .HasName("PK_Sales_SalesTerritory");

        builder.Property(st => st.TerritoryId)
            .HasColumnName("territoryid")
            .ValueGeneratedOnAdd();

        builder.Property(st => st.Name)
            .HasColumnName("name")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(st => st.CountryRegionCode)
            .HasColumnName("countryregioncode")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(st => st.Group)
            .HasColumnName("group")
            .HasColumnType("character varying")
            .IsRequired();

        // Issue #42: Final state after expand-contract — numeric(19,4)
        builder.Property(st => st.SalesYtd)
            .HasColumnName("salesytd")
            .HasColumnType("numeric(19,4)")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(st => st.SalesLastYear)
            .HasColumnName("saleslastyear")
            .HasColumnType("numeric")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(st => st.CostYtd)
            .HasColumnName("costytd")
            .HasColumnType("numeric")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(st => st.CostLastYear)
            .HasColumnName("costlastyear")
            .HasColumnType("numeric")
            .IsRequired()
            .HasDefaultValue(0m);

        builder.Property(st => st.Rowguid)
            .HasColumnName("rowguid")
            .HasDefaultValueSql("uuid_generate_v1()")
            .IsRequired();

        builder.Property(st => st.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");
    }
}
