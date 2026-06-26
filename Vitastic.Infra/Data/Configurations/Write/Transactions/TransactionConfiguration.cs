using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Transactions;

public class TransactionConfiguration : AggregateRootConfiguration<PaymentTransaction, PaymentTransactionId>
{
    public override void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("PaymentTransactions");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - PaymentTransactionId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        builder.Property(t => t.Id)
            .HasConversion(transactionId => transactionId.Value,
                value => PaymentTransactionId.CreateFrom(value).Value)
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEYS
        // ═══════════════════════════════════════════════════════════════
        //Wallet
        builder.Property(p => p.WalletId)
            .HasColumnName("WalletId")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? WalletId.CreateFrom(value.Value).Value : null
            ).IsRequired(false);
        // wallet index
        builder.HasIndex(p => p.WalletId)
            .HasDatabaseName("IX_PaymentTransactions_WalletId");
        // Order id
        builder.Property(p => p.OrderId)
            .HasColumnName("OrderId")
            .HasConversion(
                id => id != null ? id.Value : (Guid?)null,
                value => value.HasValue ? OrderId.CreateFrom(value.Value).Value : null
            ).IsRequired(false);
        // order index
        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("IX_PaymentTransactions_OrderId");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS
        // ═══════════════════════════════════════════════════════════════
        //Money
        builder.OwnsOne(p => p.Amount, money =>
        {

            money.Property(m => m.Value)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasComment("Transaction amount (can be negative for withdrawals)");
            money.Property(m => m.Currency)
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code).Value)
                .IsRequired().HasDefaultValue(Currency.IranianToman)
                .HasComment("Currency code (IRT, USD, etc.)");
            //index for amount
            money.HasIndex(p => p.Value)
                .HasDatabaseName("IX_PaymentTransactions_Amount");
        });
        // ═══════════════════════════════════════════════════════════════
        // Payment info
        // ═══════════════════════════════════════════════════════════════
        builder.OwnsOne(t=>t.PaymentInfo, paymentInfo =>
        {
            paymentInfo.Property(p => p.Authority)
                .HasColumnName("PaymentAuthority")
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Gateway transaction authority code");

            paymentInfo.Property(p => p.RefId)
                .HasColumnName("PaymentRefId")
                .IsRequired()
                .HasComment("Gateway reference ID (0 for pending)");

            paymentInfo.Property(p => p.Gateway)
                .HasColumnName("PaymentGateway")
                .HasMaxLength(50)
                .HasConversion(
                    gateway => gateway.Value,
                    value => PaymentGateway.Create(value).Value
                )
                .IsRequired()
                .HasComment("Payment gateway name (Zarinpal, Payping, etc.)");

            paymentInfo.Property(p => p.PaidDate)
                .HasColumnName("PaymentPaidDate")
                .HasColumnType("timestamptz")
                .IsRequired(false)
                .HasComment("Date when payment was completed");

            paymentInfo.Property(p => p.Description)
                .HasColumnName("PaymentDescription")
                .HasMaxLength(500)
                .IsRequired(false)
                .HasComment("Additional payment notes from gateway");
        //index for payment authority
        paymentInfo.HasIndex(p=>p.Authority)
            .HasDatabaseName("IX_PaymentTransactions_PaymentInfo_Authority");
        });
        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE PROPERTIES
        // ═══════════════════════════════════════════════════════════════
        builder.Property(t => t.Description)
            .HasColumnName("Description")
            .HasMaxLength(500)
            .IsRequired(false)
            .HasComment("User-provided transaction description");

        builder.Property(t => t.TransactionDate)
            .HasColumnName("TransactionDate")
            .HasColumnType("timestamptz")
            .IsRequired()
            .HasDefaultValueSql("NOW()")
            .HasComment("When transaction was created");

        // Index for date-based queries: "Today's transactions"
        builder.HasIndex(t => t.TransactionDate)
            .HasDatabaseName("IX_PaymentTransactions_TransactionDate");

        builder.Property(t => t.CompletedDate)
            .HasColumnName("CompletedDate")
            .HasColumnType("timestamptz")
            .IsRequired(false)
            .HasComment("When transaction was completed/failed/canceled");

        builder.Property(t => t.RevertedDate)
            .HasColumnName("RevertedDate")
            .HasColumnType("timestamptz")
            .IsRequired(false)
            .HasComment("When transaction was reverted");
        // ═══════════════════════════════════════════════════════════════
        // ENUMS
        // ═══════════════════════════════════════════════════════════════

        builder.Property(t => t.Status)
            .HasColumnName("Status")
            .HasMaxLength(20)
            .HasConversion(v=>v.ToString(),
                v=>(TransactionStatus)Enum.Parse(typeof(TransactionStatus),v))
            .IsRequired()
            .HasComment("Transaction status: Pending, Completed, Failed, Canceled, Reverted");

        // Composite index for common queries
        builder.HasIndex(t => new { t.Status, t.TransactionDate })
            .HasDatabaseName("IX_PaymentTransactions_Status_TransactionDate");
        builder.Property(t => t.Type)
            .HasColumnName("Type")
            .HasMaxLength(20)
            .HasConversion(v=>v.ToString(),
                v=>(TransactionType)Enum.Parse(typeof(TransactionType),v))
            .IsRequired()
            .HasComment("Transaction Type:  Deposit, Withdraw");

        // Composite index for common queries
        builder.HasIndex(t => new { t.Type, t.TransactionDate })
            .HasDatabaseName("IX_PaymentTransactions_Type_TransactionDate");
        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════

        builder.HasOne<Wallet>()
            .WithMany()
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(t => t.OrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES FOR PERFORMANCE
        // ═══════════════════════════════════════════════════════════════

        // Composite index for admin panel: filter by status and date
        builder.HasIndex(t => new { t.IsDeleted, t.Status, t.TransactionDate })
            .HasDatabaseName("IX_PaymentTransactions_IsDeleted_Status_TransactionDate");

        // Index for wallet transaction history
        builder.HasIndex(t => new { t.WalletId, t.TransactionDate })
            .HasDatabaseName("IX_PaymentTransactions_WalletId_TransactionDate");

        // Index for order payment lookup
        builder.HasIndex(t => new { t.OrderId, t.Status })
            .HasDatabaseName("IX_PaymentTransactions_OrderId_Status");
    }
}
