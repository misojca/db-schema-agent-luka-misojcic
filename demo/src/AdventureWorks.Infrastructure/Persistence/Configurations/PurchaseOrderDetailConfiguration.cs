using AdventureWorks.Domain.Purchasing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
    {
        builder.ToTable("purchaseorderdetail", "purchasing", t => t.ExcludeFromMigrations());

        // Composite primary key: (purchaseorderid, purchaseorderdetailid)
        builder.HasKey(p => new { p.PurchaseOrderId, p.PurchaseOrderDetailId })
            .HasName("PK_Purchasing_PurchaseOrderDetail");

        builder.Property(p => p.PurchaseOrderId)
            .HasColumnName("purchaseorderid")
            .IsRequired();

        builder.Property(p => p.PurchaseOrderDetailId)
            .HasColumnName("purchaseorderdetailid")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(p => p.DueDate)
            .HasColumnName("duedate")
            .HasColumnType("timestamp without time zone")
            .IsRequired();

        builder.Property(p => p.OrderQty)
            .HasColumnName("orderqty")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(p => p.ProductId)
            .HasColumnName("productid")
            .IsRequired();

        builder.Property(p => p.UnitPrice)
            .HasColumnName("unitprice")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(p => p.ReceivedQty)
            .HasColumnName("receivedqty")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(p => p.RejectedQty)
            .HasColumnName("rejectedqty")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(p => p.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // FK → purchasing.purchaseorderheader
        builder.HasIndex(p => p.PurchaseOrderId)
            .HasDatabaseName("IX_purchaseorderdetail_purchaseorderid");

        // FK → production.product
        builder.HasIndex(p => p.ProductId)
            .HasDatabaseName("IX_purchaseorderdetail_productid");
    }
}
