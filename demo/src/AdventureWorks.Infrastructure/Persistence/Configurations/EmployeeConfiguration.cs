using AdventureWorks.Domain.HumanResources.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdventureWorks.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table pre-existed in DB — excluded from migrations for baseline.
        // Pay columns are added via hand-written expand-contract migrations.
        builder.ToTable("employee", "humanresources", t => t.ExcludeFromMigrations());

        builder.HasKey(e => e.BusinessEntityId)
            .HasName("PK_HumanResources_Employee");

        builder.Property(e => e.BusinessEntityId)
            .HasColumnName("businessentityid")
            .ValueGeneratedNever(); // PK comes from person.businessentity

        builder.Property(e => e.NationalIdNumber)
            .HasColumnName("nationalidnumber")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(e => e.LoginId)
            .HasColumnName("loginid")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(e => e.JobTitle)
            .HasColumnName("jobtitle")
            .HasColumnType("character varying")
            .IsRequired();

        builder.Property(e => e.BirthDate)
            .HasColumnName("birthdate")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.MaritalStatus)
            .HasColumnName("maritalstatus")
            .HasColumnType("character")
            .IsRequired();

        builder.Property(e => e.Gender)
            .HasColumnName("gender")
            .HasColumnType("character")
            .IsRequired();

        builder.Property(e => e.HireDate)
            .HasColumnName("hiredate")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.SalariedFlag)
            .HasColumnName("salariedflag")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.VacationHours)
            .HasColumnName("vacationhours")
            .HasColumnType("smallint")
            .IsRequired()
            .HasDefaultValue((short)0);

        builder.Property(e => e.SickLeaveHours)
            .HasColumnName("sickleavehours")
            .HasColumnType("smallint")
            .IsRequired()
            .HasDefaultValue((short)0);

        builder.Property(e => e.CurrentFlag)
            .HasColumnName("currentflag")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(e => e.Rowguid)
            .HasColumnName("rowguid")
            .HasDefaultValueSql("uuid_generate_v1()")
            .IsRequired();

        builder.Property(e => e.ModifiedDate)
            .HasColumnName("modifieddate")
            .HasColumnType("timestamp without time zone")
            .IsRequired()
            .HasDefaultValueSql("now()");

        builder.Property(e => e.OrganizationNode)
            .HasColumnName("organizationnode")
            .HasColumnType("character varying");

        // Issue #7 — Scenario 6: pay denormalisation columns.
        // NOT NULL after Phase 2 contract migration (data migrated from employeepayhistory).
        builder.Property(e => e.CurrentPayRate)
            .HasColumnName("currentpayrate")
            .HasColumnType("numeric")
            .IsRequired();

        builder.Property(e => e.CurrentPayFrequency)
            .HasColumnName("currentpayfrequency")
            .HasColumnType("smallint")
            .IsRequired();

        builder.Property(e => e.PayChangeDate)
            .HasColumnName("paychangedate")
            .HasColumnType("timestamp without time zone")
            .IsRequired();
    }
}
