using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Courses;

public class CourseCategoryConfiguration:BaseEntityConfiguration<CourseCategory,CourseCategoryId>
{
    public override void Configure(EntityTypeBuilder<CourseCategory> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("CourseCategories");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        //Key
        builder.Property(p => p.Id)
            .HasConversion(id=>id.Value,value=>CourseCategoryId.CreateFrom(value).Value);
        // ═══════════════════════════════════════════════════════════════
        // ForeignKeys
        // ═══════════════════════════════════════════════════════════════

        builder.Property(p=>p.CourseId)
            .HasConversion(id => id.Value,value=>CourseId.CreateFrom(value).Value);
        builder.Property(p=>p.CategoryId)
            .HasConversion(id => id.Value,value=>CategoryId.CreateFrom(value).Value);
        builder.HasIndex(p=>new{p.CourseId,p.CategoryId})
            .IsUnique()
            .HasDatabaseName("UQ_CourseCategories_CourseId_CategoryId");
        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        //Description
        builder.Property(p=>p.AssignedAt)
            .HasColumnName("AssignedAt")
                .HasColumnType("timestamptz")
                .IsRequired()
                .HasDefaultValueSql("NOW()")
                .HasComment("تاریخ اتصال دوره به دسته بندی");
        // ═══════════════════════════════════════════════════════════════
        // Relations
        // ═══════════════════════════════════════════════════════════════

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(rp => rp.CategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
