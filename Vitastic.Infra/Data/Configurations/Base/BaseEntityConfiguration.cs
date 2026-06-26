using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Data.Configurations.Base
{
    public class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            //----------------------
            // Primary Key
            //----------------------
            builder.HasKey(e => e.Id);

            //----------------------
            // Audit Properties
            //----------------------
            builder.Property(e => e.CreatedBy)
                .HasColumnName("CreatedBy")
                .IsRequired()
                .HasComment("شناسه کاربر ایجادکننده");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("timestamptz")
                .IsRequired()
                .HasComment("تاریخ ایجاد");

            builder.Property(e => e.UpdatedBy)
                .HasColumnName("UpdatedBy")
                .IsRequired(false)
                .HasComment("شناسه کاربر بروزرسان");

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .HasColumnType("timestamptz")
                .IsRequired(false)
                .HasComment("تاریخ آخرین بروزرسانی");
        
            builder.HasIndex(e => e.CreatedAt)
                .HasDatabaseName($"IX_{typeof(TEntity).Name}_CreatedAt");
        }
    }
}
