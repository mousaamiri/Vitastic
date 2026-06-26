using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Tags;

public sealed class TagConfiguration : AggregateRootConfiguration<Tag, TagId>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Tags");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - TagId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => TagId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - TagName (Unique Business Key)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasMaxLength(TagName.MaxLength)
            .HasConversion(
                name => name.Value,
                value => TagName.Create(value).Value
            )
            .IsRequired();

        // Unique index on tag name (case-insensitive in DB)
        builder.HasIndex(t => t.Name)
            .IsUnique()
            .HasDatabaseName("IX_Tags_Name");

        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(t => t.UsageCount)
            .HasColumnName("UsageCount")
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Number of times this tag has been used");

        builder.Property(t => t.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Whether tag is visible and usable");

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Index for finding popular tags
        builder.HasIndex(t => t.UsageCount)
            .HasDatabaseName("IX_Tags_UsageCount")
            .IsDescending(); // Most used first

        // Composite index for active tags by usage
        builder.HasIndex(t => new { t.IsActive, t.UsageCount })
            .HasDatabaseName("IX_Tags_IsActive_UsageCount")
            .IsDescending(false, true); // IsActive ASC, UsageCount DESC

        // Index for admin filtering
        builder.HasIndex(t => new { t.IsDeleted, t.IsActive, t.UsageCount })
            .HasDatabaseName("IX_Tags_IsDeleted_IsActive_UsageCount")
            .IsDescending(false, false, true);
    }
}
