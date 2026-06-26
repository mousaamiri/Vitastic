using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Courses;

public sealed class CourseConfiguration : AggregateRootConfiguration<Course, CourseId>
{
    public override void Configure(EntityTypeBuilder<Course> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Courses");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - CourseId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => CourseId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Basic Info
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Title)
            .HasColumnName("Title")
            .HasMaxLength(CourseTitle.MaxLength)
            .HasConversion(
                title => title.Value,
                value => CourseTitle.Create(value).Value
            )
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("Description")
            .HasMaxLength(Description.MaxLength)
            .HasConversion(
                desc => desc.Value,
                value => Description.Create(value).Value
            )
            .IsRequired();

        builder.Property(c => c.ShortDescription)
            .HasColumnName("ShortDescription")
            .HasMaxLength(ShortDescription.MaxLength)
            .HasConversion(
                desc => desc.Value,
                value => ShortDescription.Create(value).Value
            )
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasColumnName("Slug")
            .HasMaxLength(Slug.MaxLength)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value).Value
            )
            .IsRequired();

        // Unique index on slug
        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Courses_Slug");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Media Files
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.ImageName)
            .HasColumnName("ImageName")
            .HasMaxLength(CourseImageName.MaxLength)
            .HasConversion(
                img => img != null ? img.Value : null,
                value => value != null ? CourseImageName.Create(value).Value : null
            )
            .IsRequired(false);

        builder.Property(c => c.ThumbnailName)
            .HasColumnName("ThumbnailName")
            .HasMaxLength(CourseThumbnailName.MaxLength)
            .HasConversion(
                thumb => thumb != null ? thumb.Value : null,
                value => value != null ? CourseThumbnailName.Create(value).Value : null
            )
            .IsRequired(false);

        builder.Property(c => c.DemoVideoName)
            .HasColumnName("DemoVideoName")
            .HasMaxLength(CourseVideoName.MaxLength)
            .HasConversion(
                video => video != null ? video.Value : null,
                value => value != null ? CourseVideoName.Create(value).Value : null
            )
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // COMPUTED PROPERTY - Price (from Episodes)
        // ═══════════════════════════════════════════════════════════════

        // Price is calculated from episodes
        // Not stored in database - computed in domain
        builder.Ignore(c => c.Price);

        // ═══════════════════════════════════════════════════════════════
        // ENUMS
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Status)
            .HasColumnName("Status")
            .HasMaxLength(20)
            .HasConversion(v=>v.ToString(),
                v=>(CourseStatus)Enum.Parse(typeof(CourseStatus),v))
            .IsRequired()
            .HasComment("Course status: Draft, Published, Archived");

        builder.Property(c => c.Level)
            .HasColumnName("Level")
            .HasMaxLength(20)
            .HasConversion(v=>v.ToString(),
                v=>(CourseLevel)Enum.Parse(typeof(CourseLevel),v))
            .IsRequired()
            .HasComment("Course level: Beginner, Intermediate, Advanced, Expert");

        // ═══════════════════════════════════════════════════════════════
        // BOOLEAN FLAGS
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.HasCertificate)
            .HasColumnName("HasCertificate")
            .IsRequired()
            .HasDefaultValue(false);

        // Is computed
        builder.Ignore(c => c.IsPublished);

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY - InstructorId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.InstructorId)
            .HasColumnName("InstructorId")
            .HasConversion(
                id => id.Value,
                value => InstructorId.CreateFrom(value).Value
            )
            .IsRequired();

        // Index for instructor's courses
        builder.HasIndex(c => c.InstructorId)
            .HasDatabaseName("IX_Courses_InstructorId");

        // Relationship to Instructor
        builder.HasOne<Instructor>()
            .WithMany()
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        // ═══════════════════════════════════════════════════════════════
        // DATE PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.PublishedAt)
            .HasColumnName("PublishedAt")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(c => c.ArchivedAt)
            .HasColumnName("ArchivedAt")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS - Sections
        // ═══════════════════════════════════════════════════════════════

        builder.HasMany(c => c.Sections)
            .WithOne()
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(c => c.Tags)
            .WithOne()
            .HasForeignKey(ct => ct.CourseId)  // ← این خط مهمه
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(c => c.Categories)
            .WithOne()
            .HasForeignKey(cc => cc.CourseId)  // ← این خط مهمه
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Categories)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        // ═══════════════════════════════════════════════════════════════
        // INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Index for published courses
        builder.HasIndex(c => new {c.Status })
            .HasDatabaseName("IX_Courses_IsPublished_Status");

        // Index for course searches by title
        builder.HasIndex(c => c.Title)
            .HasDatabaseName("IX_Courses_Title");

        // Composite index for filtering
        builder.HasIndex(c => new { c.IsDeleted, c.Status, c.CreatedAt })
            .HasDatabaseName("IX_Courses_IsDeleted_Status_CreatedAt");
    }
}
