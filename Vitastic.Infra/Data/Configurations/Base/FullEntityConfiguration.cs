using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Data.Configurations.Base;

public class FullEntityConfiguration<TEntity, TKey> : BaseEntityConfiguration<TEntity,TKey>
    where TEntity : FullEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        //----------------------
        // Base Configuration - Apply common configurations for all entities
        //----------------------
        base.Configure(builder);

        //----------------------
        // SoftDelete Properties
        //----------------------
        builder.Property(e => e.IsDeleted)
            .HasColumnName("IsDeleted")
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("وضعیت حذف");

        builder.Property(e => e.DeletedAt)
            .HasColumnName("DeletedAt")
            .HasColumnType("timestamptz")
            .IsRequired(false)
            .HasComment("تاریخ حذف");

        builder.Property(e => e.DeletedBy)
            .HasColumnName("DeletedBy")
            .IsRequired(false)
            .HasComment("شناسه کاربر حذف‌کننده");

        //--------------------
        // Query Filter for SoftDelete
        //-------------------
        builder.HasQueryFilter(e => !e.IsDeleted);

        //--------------------
        // Index for optimization
        //-------------------
        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted");

        builder.HasIndex(e=>new{e.IsDeleted,e.CreatedAt})
            .HasDatabaseName($"IX_{typeof(TEntity).Name}_IsDeleted_CreatedAt");
    }
}
