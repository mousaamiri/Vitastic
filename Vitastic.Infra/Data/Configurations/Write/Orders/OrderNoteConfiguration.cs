using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Orders;

public class OrderNoteConfiguration : FullEntityConfiguration<OrderNote, OrderNoteId>
{
    public override void Configure(EntityTypeBuilder<OrderNote> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("OrderNotes");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - OrderNoteId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => OrderNoteId.CreateFrom(value).Value
            )
            .IsRequired();
        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS
        // ═══════════════════════════════════════════════════════════════
        builder.Property(o => o.Content)
            .HasColumnName("Content")
            .HasMaxLength(OrderNote.MaxContentLength)
            .IsRequired();
        builder.Property(o => o.Type)
            .HasColumnName("Type")
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue(NoteType.System);
        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY
        // ═══════════════════════════════════════════════════════════════

        builder.Property(o => o.OrderId)
            .HasColumnName("OrderId")
            .HasConversion(id=>id.Value,value => OrderId.CreateFrom(value).Value)
            .IsRequired();
        builder.HasIndex(n => n.OrderId);
        // ═══════════════════════════════════════════════════════════════
        // RELATION
        // ═══════════════════════════════════════════════════════════════

        builder.HasOne<Order>()
            .WithMany(o => o.Notes)
            .HasForeignKey(o => o.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
