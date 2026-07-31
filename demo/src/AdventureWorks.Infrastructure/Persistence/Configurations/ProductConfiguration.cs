using AdventureWorks.Domain.Production.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("product", "production");
        builder.HasKey(p => p.ProductId).HasName("PK_Production_Product");
        builder.Property(p => p.ProductId).HasColumnName("productid").ValueGeneratedOnAdd();
        builder.Property(p => p.Name).HasColumnName("name").HasColumnType("character varying").IsRequired();
        builder.Property(p => p.ProductNumber).HasColumnName("productnumber").HasColumnType("character varying").IsRequired();
        builder.Property(p => p.MakeFlag).HasColumnName("makeflag").IsRequired().HasDefaultValue(true);
        builder.Property(p => p.FinishedGoodsFlag).HasColumnName("finishedgoodsflag").IsRequired().HasDefaultValue(true);
        builder.Property(p => p.Color).HasColumnName("color").HasColumnType("character varying");
        builder.Property(p => p.SafetyStockLevel).HasColumnName("safetystocklevel").HasColumnType("smallint").IsRequired();
        builder.Property(p => p.ReorderPoint).HasColumnName("reorderpoint").HasColumnType("smallint").IsRequired();
        builder.Property(p => p.StandardCost).HasColumnName("standardcost").HasColumnType("numeric").IsRequired();
        builder.Property(p => p.ListPrice).HasColumnName("listprice").HasColumnType("numeric").IsRequired();
        builder.Property(p => p.Size).HasColumnName("size").HasColumnType("character varying");
        builder.Property(p => p.SizeUnitMeasureCode).HasColumnName("sizeunitmeasurecode").HasColumnType("character");
        builder.Property(p => p.WeightUnitMeasureCode).HasColumnName("weightunitmeasurecode").HasColumnType("character");
        builder.Property(p => p.Weight).HasColumnName("weight").HasColumnType("numeric");
        builder.Property(p => p.DaysToManufacture).HasColumnName("daystomanufacture").IsRequired();
        builder.Property(p => p.ProductLine).HasColumnName("productline").HasColumnType("character");
        builder.Property(p => p.Class).HasColumnName("class").HasColumnType("character");
        builder.Property(p => p.Style).HasColumnName("style").HasColumnType("character");
        builder.Property(p => p.ProductSubcategoryId).HasColumnName("productsubcategoryid");
        builder.Property(p => p.ProductModelId).HasColumnName("productmodelid");
        builder.Property(p => p.SellStartDate).HasColumnName("sellstartdate").HasColumnType("timestamp without time zone").IsRequired();
        builder.Property(p => p.SellEndDate).HasColumnName("sellenddate").HasColumnType("timestamp without time zone");
        builder.Property(p => p.DiscontinuedDate).HasColumnName("discontinueddate").HasColumnType("timestamp without time zone");
        builder.Property(p => p.Rowguid).HasColumnName("rowguid").HasDefaultValueSql("uuid_generate_v1()").IsRequired();
        builder.Property(p => p.ModifiedDate).HasColumnName("modifieddate").HasColumnType("timestamp without time zone").IsRequired().HasDefaultValueSql("now()");
        builder.Property(p => p.WeightKg).HasColumnName("weight_kg").HasColumnType("numeric(10,4)").HasDefaultValue(0.0m);
    }
}
