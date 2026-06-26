// Vitastic.Infra/Data/Configurations/Write/Carts/CartConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Carts;

#region CartConfiguration

public sealed class CartConfiguration : AggregateRootConfiguration<Cart, CartId>
{
    public override void Configure(EntityTypeBuilder<Cart> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Carts");

        // ═══════════════════════════════════════════════════════════════
        // Primary key
        // ═══════════════════════════════════════════════════════════════
        builder.Property(c => c.Id)
            .HasColumnName("Id")
            .HasConversion(id => id.Value, value => CartId.CreateFrom(value).Value)
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // Foreign key - nullable for guest carts
        // ═══════════════════════════════════════════════════════════════
        builder.Property(c => c.UserId)
            .HasColumnName("UserId")
            .HasConversion(id => id!.Value, value => UserId.CreateFrom(value).Value)
            .IsRequired(false);

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);

        builder.HasIndex(c => c.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Carts_UserId")
            .HasFilter("\"UserId\" IS NOT NULL");

        // ═══════════════════════════════════════════════════════════════
        // Session ID for guest carts
        // ═══════════════════════════════════════════════════════════════
        builder.Property(c => c.SessionId)
            .HasColumnName("SessionId")
            .HasMaxLength(128)
            .IsRequired(false);

        builder.HasIndex(c => c.SessionId)
            .HasDatabaseName("IX_Carts_SessionId")
            .HasFilter("\"SessionId\" IS NOT NULL");

        // ═══════════════════════════════════════════════════════════════
        // Snapshot value objects
        // ═══════════════════════════════════════════════════════════════
        builder.Property(c => c.UserFullName)
            .HasColumnName("UserFullName")
            .HasMaxLength(FullName.MaxLength)
            .HasConversion(name => name!.Value, value => FullName.Create(value).Value)
            .IsRequired(false);

        builder.Property(c => c.UserEmail)
            .HasColumnName("UserEmail")
            .HasMaxLength(Email.MaxLength)
            .HasConversion(email => email!.Value, value => Email.Create(value).Value)
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // Computed properties - not persisted
        // ═══════════════════════════════════════════════════════════════
        builder.Ignore(c => c.ItemsTotal);
        builder.Ignore(c => c.ItemsCount);
        builder.Ignore(c => c.IsGuest);

        // ═══════════════════════════════════════════════════════════════
        // Cart items collection
        // ═══════════════════════════════════════════════════════════════
        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_items");

        // ═══════════════════════════════════════════════════════════════
        // Composite indexes
        // ═══════════════════════════════════════════════════════════════
        builder.HasIndex(c => new { c.IsDeleted, c.UserId })
            .HasDatabaseName("IX_Carts_IsDeleted_UserId");

        builder.HasIndex(c => new { c.IsDeleted, c.SessionId, c.CreatedAt })
            .HasDatabaseName("IX_Carts_IsDeleted_SessionId_CreatedAt");
        // ═══════════════════════════════════════════════════════════════
        // Either UserId or SessionId must be set, not both
        // ═══════════════════════════════════════════════════════════════
        builder.HasCheckConstraint(
            "CK_Carts_UserOrSession",
            "(\"UserId\" IS NOT NULL AND \"SessionId\" IS NULL) OR (\"UserId\" IS NULL AND \"SessionId\" IS NOT NULL)"
        );
    }
}

#endregion
