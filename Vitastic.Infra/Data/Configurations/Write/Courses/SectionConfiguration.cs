using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Courses;

public sealed class SectionConfiguration : FullEntityConfiguration<Section, SectionId>
{
    public override void Configure(EntityTypeBuilder<Section> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Sections");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - SectionId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(s => s.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => SectionId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY - CourseId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(s => s.CourseId)
            .HasColumnName("CourseId")
            .HasConversion(
                id => id.Value,
                value => CourseId.CreateFrom(value).Value
            )
            .IsRequired();

        // Index for course sections
        builder.HasIndex(s => s.CourseId)
            .HasDatabaseName("IX_Sections_CourseId");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - SectionTitle
        // ═══════════════════════════════════════════════════════════════

        builder.Property(s => s.Title)
            .HasColumnName("Title")
            .HasMaxLength(SectionTitle.MaxLength)
            .HasConversion(
                title => title.Value,
                value => SectionTitle.Create(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(s => s.DisplayOrder)
            .HasColumnName("DisplayOrder")
            .IsRequired()
            .HasComment("Order of section within course");

        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════
        builder.HasOne<Course>()
            .WithMany(c => c.Sections)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Episodes)
            .WithOne()
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ═══════════════════════════════════════════════════════════════
        // INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Composite index for ordering sections in course
        builder.HasIndex(s => new { s.CourseId, s.DisplayOrder })
            .HasDatabaseName("IX_Sections_CourseId_DisplayOrder");

        // Index for section title searches within course
        builder.HasIndex(s => new { s.CourseId, s.Title })
            .HasDatabaseName("IX_Sections_CourseId_Title");
    }
}
