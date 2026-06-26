using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Courses;

public class CourseTagConfiguration:BaseEntityConfiguration<CourseTag,CourseTagId>
{
    public override void Configure(EntityTypeBuilder<CourseTag> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("CourseTags");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        //Key
        builder.Property(p => p.Id)
            .HasConversion(id=>id.Value,value=>CourseTagId.CreateFrom(value).Value);
        // ═══════════════════════════════════════════════════════════════
        // ForeignKeys
        // ═══════════════════════════════════════════════════════════════

        builder.Property(p=>p.CourseId)
            .HasConversion(id => id.Value,value=>CourseId.CreateFrom(value).Value);
        builder.Property(p=>p.TagId)
            .HasConversion(id => id.Value,value=>TagId.CreateFrom(value).Value);
        builder.HasIndex(p=>new{p.CourseId,p.TagId})
            .IsUnique()
            .HasDatabaseName("UQ_CourseTags_CourseId_TagId");
        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        //Description
        builder.Property(p=>p.AssignedAt)
            .HasColumnName("AssignedAt")
                .HasColumnType("timestamptz")
                .IsRequired()
                .HasDefaultValueSql("NOW()")
                .HasComment("تاریخ اتصال دوره به برچسب");
        // ═══════════════════════════════════════════════════════════════
        // Relations
        // ═══════════════════════════════════════════════════════════════

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(rp => rp.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
