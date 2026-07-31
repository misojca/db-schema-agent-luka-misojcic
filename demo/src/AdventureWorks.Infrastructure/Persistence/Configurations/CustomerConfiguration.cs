using AdventureWorks.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customer", "sales", t => t.ExcludeFromMigrations());

        builder.HasKey(c => c.CustomerId);

        builder.Property(c => c.CustomerId)
            .HasColumnName("customerid")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.PersonId)
            .HasColumnName("personid");

        builder.Property(c => c.StoreId)
            .HasColumnName("storeid");

        builder.Property(c => c.TerritoryId)
            .HasColumnName("territoryid");

        builder.Property(c => c.Rowguid)
            .HasColumnName("rowguid")
            .IsRequired();

        builder.Property(c => c.ModifiedDate)
            .HasColumnName("modifieddate")
            .IsRequired();
    }
}
