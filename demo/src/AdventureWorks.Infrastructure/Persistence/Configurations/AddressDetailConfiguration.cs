using AdventureWorks.Domain.Person.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class AddressDetailConfiguration : IEntityTypeConfiguration<AddressDetail>
{
    public void Configure(EntityTypeBuilder<AddressDetail> builder)
    {
        // New table created by expand-contract migration (issue #6, Scenario 5).
        builder.ToTable("addressdetail", "person");

        // PK is also FK to person.address — one detail row per address (1:1)
        builder.HasKey(d => d.AddressId)
            .HasName("PK_Person_AddressDetail");

        builder.Property(d => d.AddressId)
            .HasColumnName("addressid")
            .ValueGeneratedNever(); // PK comes from address, not generated here

        // Phase 1: nullable during data migration. Phase 2: all rows populated.
        builder.Property(d => d.SpatialLocation)
            .HasColumnName("spatiallocation")
            .HasColumnType("bytea");

        builder.Property(d => d.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // FK → person.address (cross-table within same schema)
        // Enforced via PK reuse pattern; index not needed (PK is the index)
    }
}
