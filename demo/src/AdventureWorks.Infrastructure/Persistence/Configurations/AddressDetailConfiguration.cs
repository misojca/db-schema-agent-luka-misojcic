using AdventureWorks.Domain.Person.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class AddressDetailConfiguration : IEntityTypeConfiguration<AddressDetail>
{
    public void Configure(EntityTypeBuilder<AddressDetail> builder)
    {
        // Phase 2 (contract): table already created in Phase 1 migration.
        // Exclude from migrations so EF doesn't try to recreate it.
        builder.ToTable("addressdetail", "person", t => t.ExcludeFromMigrations());

        // PK is also FK to person.address — one detail row per address (1:1)
        builder.HasKey(d => d.AddressId)
            .HasName("PK_Person_AddressDetail");

        builder.Property(d => d.AddressId)
            .HasColumnName("addressid")
            .ValueGeneratedNever(); // PK comes from address, not generated here

        builder.Property(d => d.SpatialLocation)
            .HasColumnName("spatiallocation")
            .HasColumnType("bytea");

        builder.Property(d => d.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // FK → person.address is configured on the Address side (AddressConfiguration)
        // to keep the one-to-one relationship with navigation property.
    }
}
