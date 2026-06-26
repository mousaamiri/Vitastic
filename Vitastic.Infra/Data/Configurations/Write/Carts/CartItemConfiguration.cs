using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Carts;

public sealed class CartItemConfiguration : FullEntityConfiguration<CartItem, CartItemId>
{
    public override void Configure(EntityTypeBuilder<CartItem> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("CartItems");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - CartItemId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => CartItemId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY - CartId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.CartId)
            .HasColumnName("CartId")
            .HasConversion(
                id => id.Value,
                value => CartId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY - CourseId (Reference, not navigation)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.CourseId)
            .HasColumnName("CourseId")
            .HasConversion(
                id => id.Value,
                value => CourseId.CreateFrom(value).Value
            )
            .IsRequired();

        // Reference to Course (no cascade - course deletion shouldn't auto-remove cart items)
        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(i => i.CourseId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        // Unique constraint - one course per cart
        builder.HasIndex(i => new { i.CartId, i.CourseId })
            .IsUnique()
            .HasDatabaseName("IX_CartItems_CartId_CourseId_Unique");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - CourseTitle (Snapshot)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.CourseTitle)
            .HasColumnName("CourseTitle")
            .HasMaxLength(CourseTitle.MaxLength)
            .HasConversion(
                title => title.Value,
                value => CourseTitle.Create(value).Value
            )
            .IsRequired()
            .HasComment("Snapshot of course title when added to cart");
        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - CourseInstructorName (Snapshot)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.CourseInstructorName)
            .HasColumnName("CourseInstructorName")
            .HasMaxLength(FullName.MaxLength)
            .HasConversion(
                courseInstructorName => courseInstructorName.Value,
                value => FullName.Create(value).Value
            )
            .IsRequired()
            .HasComment("Snapshot of Course Instructor Name when added to cart");
        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - CourseImageName (Snapshot, Optional)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.CourseImageName)
            .HasColumnName("CourseImageName")
            .HasMaxLength(CourseImageName.MaxLength)
            .HasConversion(
                img => img != null ? img.Value : null,
                value => value != null ? CourseImageName.Create(value).Value : null
            )
            .IsRequired(false)
            .HasComment("Snapshot of course image when added to cart");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - UnitPrice (Money - Owned Type)
        // ═══════════════════════════════════════════════════════════════

        builder.OwnsOne(i => i.UnitPrice, money =>
        {
            money.Property(m => m.Value)
                .HasColumnName("UnitPrice")
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasComment("Snapshot of course price when added to cart");

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired()
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                )
                .HasComment("Currency code (e.g., IRT, IRR, USD)");
        });

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Index for querying items by cart
        builder.HasIndex(i => i.CartId)
            .HasDatabaseName("IX_CartItems_CartId");

        // Index for checking if a course exists in any cart
        builder.HasIndex(i => i.CourseId)
            .HasDatabaseName("IX_CartItems_CourseId");
    }
}
