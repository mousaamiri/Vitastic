using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Courses;

public sealed class EpisodeConfiguration : FullEntityConfiguration<Episode, EpisodeId>
{
    public override void Configure(EntityTypeBuilder<Episode> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Episodes");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - EpisodeId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(e => e.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => EpisodeId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // FOREIGN KEY - SectionId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(e => e.SectionId)
            .HasColumnName("SectionId")
            .HasConversion(
                id => id.Value,
                value => SectionId.CreateFrom(value).Value
            )
            .IsRequired();

        // Index for section episodes
        builder.HasIndex(e => e.SectionId)
            .HasDatabaseName("IX_Episodes_SectionId");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - EpisodeTitle
        // ═══════════════════════════════════════════════════════════════

        builder.Property(e => e.Title)
            .HasColumnName("Title")
            .HasMaxLength(EpisodeTitle.MaxLength)
            .HasConversion(
                title => title.Value,
                value => EpisodeTitle.Create(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - EpisodeVideoName
        // ═══════════════════════════════════════════════════════════════

        builder.Property(e => e.VideoFileName)
            .HasColumnName("VideoFileName")
            .HasMaxLength(EpisodeVideoName.MaxLength)
            .HasConversion(
                video => video != null ? video.Value : null,
                value => value != null ? EpisodeVideoName.Create(value).Value : null
            )
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(e => e.Duration)
            .HasColumnName("Duration")
            .IsRequired()
            .HasComment("Episode duration in ticks (TimeSpan)");

        builder.Property(e => e.DisplayOrder)
            .HasColumnName("DisplayOrder")
            .IsRequired()
            .HasComment("Order of episode within section");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - Money (Price)
        // ═══════════════════════════════════════════════════════════════

        builder.OwnsOne(e => e.Price, price =>
        {
            price.Property(m => m.Value)
                .HasColumnName("Price")
                .HasColumnType("decimal(18,2)")
                .IsRequired()
                .HasDefaultValue(0)
                .HasComment("Episode price (0 = free)");

            price.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                ).HasDefaultValue(Currency.IranianToman)
                .IsRequired();

            // Index for free episodes queries
            price.HasIndex(m => m.Value)
                .HasDatabaseName("IX_Episodes_Price")
                .HasFilter("\"Price\" = 0");


        });

        // ═══════════════════════════════════════════════════════════════
        // COMPUTED COLUMN - IsFree
        // ═══════════════════════════════════════════════════════════════


        // IsFree is computed from Price.Amount
        // Not stored in database - calculated in domain
        builder.Ignore(e => e.IsFree);

        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════

        builder.HasOne<Section>()
            .WithMany(s => s.Episodes)
            .HasForeignKey(e => e.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // ═══════════════════════════════════════════════════════════════
        // INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Composite index for ordering episodes in section
        builder.HasIndex(e => new { e.SectionId, e.DisplayOrder })
            .HasDatabaseName("IX_Episodes_SectionId_DisplayOrder");

    }
}
