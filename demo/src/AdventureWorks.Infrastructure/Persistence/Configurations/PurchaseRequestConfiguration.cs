using AdventureWorks.Domain.Purchasing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for purchasing.purchaserequest.
/// New table — EF Core manages full DDL via migration AddPurchaseRequest.
/// Cross-schema FKs:
///   employeeid → humanresources.employee.businessentityid
///   productid  → production.product.productid
/// </summary>
public class PurchaseRequestConfiguration : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.ToTable("purchaserequest", "purchasing");

        builder.HasKey(r => r.PurchaseRequestId)
            .HasName("PK_Purchasing_PurchaseRequest");

        builder.Property(r => r.PurchaseRequestId)
            .HasColumnName("purchaserequestid")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.EmployeeId)
            .HasColumnName("employeeid")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(r => r.ProductId)
            .HasColumnName("productid")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(r => r.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(r => r.RequestDate)
            .HasColumnName("requestdate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(r => r.Status)
            .HasColumnName("status")
            .HasColumnType("character varying(20)")
            .IsRequired()
            .HasDefaultValue("pending");

        // Index on FK → humanresources.employee.businessentityid
        builder.HasIndex(r => r.EmployeeId)
            .HasDatabaseName("IX_purchaserequest_employeeid");

        // Index on FK → production.product.productid
        builder.HasIndex(r => r.ProductId)
            .HasDatabaseName("IX_purchaserequest_productid");
    }
}
