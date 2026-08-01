using AdventureWorks.Domain.Person.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        // Table pre-existed in DB — excluded from migrations for baseline.
        builder.ToTable("address", "person", t => t.ExcludeFromMigrations());

        builder.HasKey(a => a.AddressId)
            .HasName("PK_Person_Address");

        builder.Property(a => a.AddressId)
            .HasColumnName("addressid")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.AddressLine1)
            .HasColumnName("addressline1")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(a => a.AddressLine2)
            .HasColumnName("addressline2")
            .HasColumnType("character varying");

        builder.Property(a => a.City)
            .HasColumnName("city")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(a => a.StateProvinceId)
            .HasColumnName("stateprovinceid")
            .IsRequired();

        builder.Property(a => a.PostalCode)
            .HasColumnName("postalcode")
            .HasColumnType("character varying")
            .IsRequired();

        // Issue #6 Phase 1: spatiallocation still on address during expand phase.
        // Removed from this config in Phase 2 (contract).
        builder.Property(a => a.SpatialLocation)
            .HasColumnName("spatiallocation")
            .HasColumnType("bytea");

        builder.Property(a => a.Rowguid)
            .HasColumnName("rowguid")
            .HasDefaultValueSql("uuid_generate_v1()")
            .IsRequired();

        builder.Property(a => a.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Index on FK from other tables referencing address
        builder.HasIndex(a => a.StateProvinceId)
            .HasDatabaseName("IX_address_stateprovinceid");
    }
}
