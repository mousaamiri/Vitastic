using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Wallets;

public class WalletConfiguration : AggregateRootConfiguration<Wallet, WalletId>
{
    public override void Configure(EntityTypeBuilder<Wallet> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("Wallets");
        // ═══════════════════════════════════════
        // PRIMARY KEY
        // ═══════════════════════════════════════

        builder.Property(w => w.Id)
            .HasConversion(
                id => id.Value,
                value => WalletId.CreateFrom(value).Value
            );

        // ═══════════════════════════════════════
        // FOREIGN KEY - UserId
        // ═══════════════════════════════════════

        builder.Property(w => w.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.CreateFrom(value).Value
            );

        builder.HasIndex(w => w.UserId);

        // ═══════════════════════════════════════
        // VALUE OBJECT - Currency
        // ═══════════════════════════════════════

        builder.Property(w => w.Currency)
            .HasColumnName("Currency")
            .HasMaxLength(Currency.CodeLength)
            .HasConversion(
                currency => currency.Code,
                code => Currency.FromCode(code).Value
            ).HasDefaultValue(Currency.IranianToman)
            .IsRequired();

        // Unique: one wallet per user per currency
        builder.HasIndex(w => new { w.UserId, w.Currency })
            .IsUnique();

        // ═══════════════════════════════════════
        // VALUE OBJECT - Money (Balance)
        // ═══════════════════════════════════════

        builder.OwnsOne(w => w.Balance, balance =>
        {
            //Just Value
            balance.Property(m => m.Value)
                .HasColumnName("Balance")
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasDefaultValue(0);

            balance.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                ).HasDefaultValue(Currency.IranianToman)
                .IsRequired(false); // Currency is determined by the wallet's Currency property

            balance.HasIndex(m => m.Value)
                .HasDatabaseName("IX_Wallets_Balance");
        });


        // ═══════════════════════════════════════
        // IGNORE - TransactionIds
        // ═══════════════════════════════════════
        // This collection comes from PaymentTransaction.Id
        builder.Ignore(w => w.TransactionIds);
        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Don't cascade delete
        // ═══════════════════════════════════════════════════════════════
        // QUERY FILTERS
        // ═══════════════════════════════════════════════════════════════
        builder.HasIndex(w => new { w.IsDeleted, w.UserId, w.Currency })
            .HasDatabaseName("IX_Wallets_IsDeleted_UserId_Currency");
    }
}
