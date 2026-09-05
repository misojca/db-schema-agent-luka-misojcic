using AdventureWorks.Domain.Purchasing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

/// <summary>
/// Fluent API configuration for purchasing.purchasereturn.
/// Reference: issue #24 / Scenario 7.
///
/// Note on FK design: purchasing.purchaseorderdetail has a composite PK
/// (purchaseorderid, purchaseorderdetailid). Because this table references
/// only purchaseorderdetailid — which is de-facto unique but lacks a
/// standalone unique constraint — a database-level FK constraint is not
/// declared here to avoid a PostgreSQL constraint violation. The index on
/// purchaseorderdetailid is still created for efficient lookups. If a strict
/// DB-level FK is needed in future, add a UNIQUE INDEX on
/// purchaseorderdetail.purchaseorderdetailid first.
/// </summary>
public class PurchaseReturnConfiguration : IEntityTypeConfiguration<PurchaseReturn>
{
    public void Configure(EntityTypeBuilder<PurchaseReturn> builder)
    {
        builder.ToTable("purchasereturn", "purchasing", t => t.ExcludeFromMigrations());
        // Primary key — serial (identity)
        builder.HasKey(r => r.PurchaseReturnId)
            .HasName("PK_Purchasing_PurchaseReturn");

        builder.Property(r => r.PurchaseReturnId)
            .HasColumnName("purchasereturnid")
            .HasColumnType("integer")
            .IsRequired()
            .ValueGeneratedOnAdd();

        NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(
            builder.Property(r => r.PurchaseReturnId));

        builder.Property(r => r.PurchaseOrderDetailId)
            .HasColumnName("purchaseorderdetailid")
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(r => r.ReturnDate)
            .HasColumnName("returndate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(r => r.Quantity)
            .HasColumnName("quantity")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(r => r.Reason)
            .HasColumnName("reason")
            .HasColumnType("character varying(200)")
            .IsRequired(false);

        builder.Property(r => r.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        // Index on FK column for efficient lookups
        builder.HasIndex(r => r.PurchaseOrderDetailId)
            .HasDatabaseName("IX_purchasereturn_purchaseorderdetailid");

        // Ignore navigation property — no DB-level FK constraint
        // (see class-level comment for rationale)
        builder.Ignore(r => r.PurchaseOrderDetail);
    }
}
