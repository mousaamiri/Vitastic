using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.Enums;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Orders;

public sealed class OrderConfiguration : AggregateRootConfiguration<Order, OrderId>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Orders");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - OrderId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => OrderId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - OrderNumber (Unique Business Key)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.OrderNumber)
            .HasColumnName("OrderNumber")
            .HasMaxLength(50)
            .HasConversion(
                number => number.Value,
                value => OrderNumber.FromExisting(value)
            )
            .IsRequired();

        // Unique index on order number
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique()
            .HasDatabaseName("IX_Orders_OrderNumber");

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY - UserId (Customer)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.UserId)
            .HasColumnName("UserId")
            .HasConversion(
                id => id.Value,
                value => UserId.CreateFrom(value).Value
            )
            .IsRequired();

        // Index for user's orders
        builder.HasIndex(o => o.UserId)
            .HasDatabaseName("IX_Orders_UserId");

        // Relationship to User
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - User Information (Snapshot)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.UserFullName)
            .HasColumnName("UserFullName")
            .HasMaxLength(200)
            .HasConversion(
                name => name.Value,
                value => FullName.Create(value).Value
            )
            .IsRequired();

        builder.Property(o => o.UserEmail)
            .HasColumnName("UserEmail")
            .HasMaxLength(256)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Money (Financial Fields)
        // ═══════════════════════════════════════════════════════════════

        ConfigureMoney(builder, o => o.ItemsTotal, "ItemsTotal");
        ConfigureMoney(builder, o => o.DiscountAmount, "DiscountAmount");
        ConfigureMoney(builder, o => o.TaxAmount, "TaxAmount");
        ConfigureMoney(builder, o => o.ShippingAmount, "ShippingAmount");
        ConfigureMoney(builder, o => o.FinalAmount, "FinalAmount");

        // ═══════════════════════════════════════════════════════════════
        // ENUMS - Status & Payment Method
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.Status)
            .HasColumnName("Status")
            .HasMaxLength(20)
            .HasConversion(v => v.ToString(),
                v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v))
            .IsRequired()
            .HasComment("Order status: Pending, Processing, Completed, Cancelled, Refunded");

        builder.Property(o => o.PaymentMethod)
            .HasColumnName("PaymentMethod")
            .HasMaxLength(20)
            .HasConversion(v => v.ToString(),
                v => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), v))
            .IsRequired()
            .HasComment("Payment method: Gateway, Wallet");

        // Composite index for order filtering
        builder.HasIndex(o => new { o.Status, o.CreatedAt })
            .HasDatabaseName("IX_Orders_Status_CreatedAt");

        // ═══════════════════════════════════════════════════════════════
        // DateTimeOffsetPROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.CompletedAt)
            .HasColumnName("CompletedAt")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(o => o.CancelledAt)
            .HasColumnName("CancelledAt")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        builder.Property(o => o.PaymentDate)
            .HasColumnName("PaymentDate")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // PAYMENT INFORMATION
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.PaymentTransactionId)
            .HasColumnName("PaymentTransactionId")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? PaymentTransactionId.CreateFrom(value.Value).Value : null
            )
            .IsRequired(false);

        // Index for payment lookup
        builder.HasIndex(o => o.PaymentTransactionId)
            .HasDatabaseName("IX_Orders_PaymentTransactionId")
            .HasFilter("\"PaymentTransactionId\" IS NOT NULL");

        builder.Property(o => o.PaymentGateway)
            .HasColumnName("PaymentGateway")
            .HasMaxLength(50)
            .HasConversion(
                gateway => gateway != null ? gateway.Value : null,
                value => value != null ? PaymentGateway.Create(value).Value : null
            )
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // DISCOUNT INFORMATION
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.DiscountId)
            .HasColumnName("DiscountId")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? DiscountId.CreateFrom(value.Value).Value : null
            )
            .IsRequired(false);

        builder.Property(o => o.DiscountCode)
            .HasColumnName("DiscountCode")
            .HasMaxLength(50)
            .HasConversion(
                code => code != null ? code.Value : null,
                value => value != null ? DiscountCode.Create(value).Value : null
            )
            .IsRequired(false);

        // Index for discount usage analysis
        builder.HasIndex(o => o.DiscountId)
            .HasDatabaseName("IX_Orders_DiscountId")
            .HasFilter("\"DiscountId\" IS NOT NULL");

        // ═══════════════════════════════════════════════════════════════
        // CONTACT INFORMATION (Owned Value Objects)
        // ═══════════════════════════════════════════════════════════════

        builder.OwnsOne(o => o.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .HasColumnName("PhoneNumber")
                .HasMaxLength(20)
                .IsRequired(false);
        });

        builder.OwnsOne(o => o.BillingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("BillingStreet")
                .HasMaxLength(Address.MaxStreetLength)
                .IsRequired(false);

            address.Property(a => a.City)
                .HasColumnName("BillingCity")
                .HasMaxLength(Address.MaxCityLength)
                .IsRequired(false);

            address.Property(a => a.State)
                .HasColumnName("BillingState")
                .HasMaxLength(Address.MaxStateLength)
                .IsRequired(false);

            address.Property(a => a.Country)
                .HasColumnName("BillingCountry")
                .HasMaxLength(Address.MaxCountryLength)
                .IsRequired(false);

            address.Property(a => a.PostalCode)
                .HasColumnName("BillingPostalCode")
                .HasMaxLength(Address.PostalCodeLength)
                .IsRequired(false);

            address.Property(a => a.AdditionalInfo)
                .HasColumnName("BillingAdditionalInfo")
                .HasMaxLength(Address.MaxAdditionalInfoLength)
                .IsRequired(false);
        });

        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("ShippingStreet")
                .HasMaxLength(Address.MaxStreetLength)
                .IsRequired(false);

            address.Property(a => a.City)
                .HasColumnName("ShippingCity")
                .HasMaxLength(Address.MaxCityLength)
                .IsRequired(false);

            address.Property(a => a.State)
                .HasColumnName("ShippingState")
                .HasMaxLength(Address.MaxStateLength)
                .IsRequired(false);

            address.Property(a => a.Country)
                .HasColumnName("ShippingCountry")
                .HasMaxLength(Address.MaxCountryLength)
                .IsRequired(false);

            address.Property(a => a.PostalCode)
                .HasColumnName("ShippingPostalCode")
                .HasMaxLength(Address.PostalCodeLength)
                .IsRequired(false);

            address.Property(a => a.AdditionalInfo)
                .HasColumnName("ShippingAdditionalInfo")
                .HasMaxLength(Address.MaxAdditionalInfoLength)
                .IsRequired(false);
        });

        // ═══════════════════════════════════════════════════════════════
        // NOTES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.CustomerNote)
            .HasColumnName("CustomerNote")
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(o => o.AdminNote)
            .HasColumnName("AdminNote")
            .HasMaxLength(1000)
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS - Order Items
        // ═══════════════════════════════════════════════════════════════

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.Notes)
            .WithOne()
            .HasForeignKey(n => n.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Index for user order history
        builder.HasIndex(o => new { o.UserId, o.CreatedAt })
            .HasDatabaseName("IX_Orders_UserId_CreatedAt");

        // Index for admin panel filtering
        builder.HasIndex(o => new { o.IsDeleted, o.Status, o.CreatedAt })
            .HasDatabaseName("IX_Orders_IsDeleted_Status_CreatedAt");
    }

    /// <summary>
    /// Helper method to configure Money value objects consistently
    /// </summary>
    private void ConfigureMoney(
        EntityTypeBuilder<Order> builder,
        Expression<Func<Order, Money>> propertyExpression,
        string columnPrefix)
        //decimal? defaultValue = null)
    {
        builder.Property(propertyExpression)
            .HasColumnName(columnPrefix)
            .HasColumnType("decimal(18,2)")
            .HasConversion(
                m => m.Value,
                v => Money.Create(v, Currency.IranianToman).Value
            )
            .IsRequired();

        // builder.Property($"{columnPrefix}Currency")
        //     .HasMaxLength(3)
        //     .HasDefaultValue("IRT")
        //     .IsRequired();
    }
}
