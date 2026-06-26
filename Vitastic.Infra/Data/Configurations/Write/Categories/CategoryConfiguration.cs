using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Categories;

public sealed class CategoryConfiguration : AggregateRootConfiguration<Category, CategoryId>
{
    public override void Configure(EntityTypeBuilder<Category> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Categories");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - CategoryId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => CategoryId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - CategoryName
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Name)
            .HasColumnName("Name")
            .HasMaxLength(CategoryName.MaxLength)
            .HasConversion(
                name => name.Value,
                value => CategoryName.Create(value).Value
            )
            .IsRequired();
        //Category name must be unique
        builder.HasIndex(c => c.Name).IsUnique();
        // Index for name searches
        builder.HasIndex(c => c.Name)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Name_Unique");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - Slug (Unique Business Key)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Slug)
            .HasColumnName("Slug")
            .HasMaxLength(Slug.MaxLength)
            .HasConversion(
                slug => slug.Value,
                value => Slug.Create(value).Value
            )
            .IsRequired();

        // Unique index on slug (used in URLs)
        builder.HasIndex(c => c.Slug)
            .IsUnique()
            .HasDatabaseName("IX_Categories_Slug");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - Description (Optional)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.Description)
            .HasColumnName("Description")
            .HasMaxLength(Description.MaxLength)
            .HasConversion(
                desc => desc != null ? desc.Value : null,
                value => value != null ? Description.Create(value).Value : null
            )
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // HIERARCHICAL STRUCTURE - Parent Category (Self-Reference)
        // ═══════════════════════════════════════════════════════════════


        // ParentCategoryId - Optional (null = root category)
        // Enables multi-level category hierarchy
        builder.Property(c => c.ParentCategoryId)
            .HasColumnName("ParentCategoryId")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? CategoryId.CreateFrom(value.Value).Value : null
            )
            .IsRequired(false);

        // Self-referencing relationship
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Index for parent queries (e.g., get all subcategories)
        builder.HasIndex(c => c.ParentCategoryId)
            .HasDatabaseName("IX_Categories_ParentCategoryId")
            .HasFilter("\"ParentCategoryId\" IS NOT NULL");

        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(c => c.DisplayOrder)
            .HasColumnName("DisplayOrder")
            .IsRequired()
            .HasDefaultValue(1)
            .HasComment("Order for displaying categories in UI");

        builder.Property(c => c.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Whether category is visible to users");

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Composite index for displaying categories
        builder.HasIndex(c => new { c.IsActive, c.DisplayOrder })
            .HasDatabaseName("IX_Categories_IsActive_DisplayOrder");

        // Index for root categories (no parent)
        builder.HasIndex(c => new { c.ParentCategoryId, c.DisplayOrder })
            .HasDatabaseName("IX_Categories_ParentCategoryId_DisplayOrder")
            .HasFilter("\"ParentCategoryId\" IS NULL");

        // Composite index for admin filtering
        builder.HasIndex(c => new { c.IsDeleted, c.IsActive, c.DisplayOrder })
            .HasDatabaseName("IX_Categories_IsDeleted_IsActive_DisplayOrder");
    }
}
