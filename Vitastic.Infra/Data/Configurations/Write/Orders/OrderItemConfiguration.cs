using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Orders;

public sealed class OrderItemConfiguration : FullEntityConfiguration<OrderItem, OrderItemId>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("OrderItems");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - OrderItemId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => OrderItemId.CreateFrom(value).Value
            )
            .IsRequired();
        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Course Information
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.CourseTitle)
            .HasColumnName("CourseTitle")
            .HasMaxLength(CourseTitle.MaxLength)
            .HasConversion(
                title => title.Value,
                value => CourseTitle.Create(value).Value
            )
            .IsRequired();

        builder.Property(c => c.CourseImageName)
            .HasColumnName("ImageName")
            .HasMaxLength(CourseImageName.MaxLength)
            .HasConversion(
                img => img != null ? img.Value : null,
                value => value != null ? CourseImageName.Create(value).Value : null
            )
            .IsRequired(false);

        builder.Property(i => i.InstructorFullName)
            .HasColumnName("InstructorFullName")
            .HasMaxLength(200)
            .HasConversion(
                name => name.Value,
                value => FullName.Create(value).Value
            )
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Money (Prices)
        // ═══════════════════════════════════════════════════════════════
        ConfigureMoney(builder, o => o.UnitPrice, "UnitPrice");
        ConfigureMoney(builder, o => o.DiscountAmount, "DiscountAmount", defaultValue: 0);
        ConfigureMoney(builder, o => o.FinalPrice, "FinalPrice", defaultValue: 0);

        // ═══════════════════════════════════════════════════════════════
        // ACCESS MANAGEMENT PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.IsAccessGranted)
            .HasColumnName("IsAccessGranted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(i => i.AccessGrantedAt)
            .HasColumnName("AccessGrantedAt")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(i => i.AccessRevokedAt)
            .HasColumnName("AccessRevokedAt")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(i => i.AccessExpiryDate)
            .HasColumnName("AccessExpiryDate")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        // Index for access queries
        builder.HasIndex(i => new { i.IsAccessGranted, i.AccessExpiryDate })
            .HasDatabaseName("IX_OrderItems_IsAccessGranted_AccessExpiryDate");

        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════

        builder.HasOne<Order>()
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Course>()
            .WithMany()
            .HasForeignKey(i => i.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Composite index for user's purchased courses
        builder.HasIndex(i => new { i.OrderId, i.CourseId })
            .HasDatabaseName("IX_OrderItems_OrderId_CourseId");

        // Index for course sales analysis
        builder.HasIndex(i => new { i.CourseId, i.IsDeleted })
            .HasDatabaseName("IX_OrderItems_CourseId_IsDeleted");
    }
    /// <summary>
    /// Helper method to configure Money value objects consistently
    /// </summary>
    private void ConfigureMoney(
        EntityTypeBuilder<OrderItem> builder,
        Expression<Func<OrderItem, Money>> propertyExpression,
        string columnPrefix,
        decimal? defaultValue = null)
    {
        builder.OwnsOne(propertyExpression!, money =>
        {
            PropertyBuilder<decimal> amountConfig = money.Property(m => m.Value)
                .HasColumnName($"{columnPrefix}")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            if (defaultValue.HasValue)
                amountConfig.HasDefaultValue(defaultValue.Value);

            money.Property(m => m.Currency)
                .HasColumnName($"{columnPrefix}Currency")
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                ).HasDefaultValue(Currency.IranianToman)
                .IsRequired();
        });
    }

}
