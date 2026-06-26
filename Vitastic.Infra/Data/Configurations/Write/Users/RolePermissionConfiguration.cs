using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Users;

public class RolePermissionConfiguration:BaseEntityConfiguration<RolePermission,RolePermissionId>
{
    public override void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("RolePermissions");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        //Key
        builder.Property(p => p.Id)
            .HasConversion(id=>id.Value,value=>RolePermissionId.CreateFrom(value).Value);
        // ═══════════════════════════════════════════════════════════════
        // ForeignKeys
        // ═══════════════════════════════════════════════════════════════

        builder.Property(p=>p.RoleId)
            .HasConversion(id => id.Value,value=>RoleId.CreateFrom(value).Value);
        builder.Property(p=>p.PermissionId)
            .HasConversion(id => id.Value,value=>PermissionId.CreateFrom(value).Value);
        builder.HasIndex(p=>new{p.RoleId,p.PermissionId})
            .IsUnique()
            .HasDatabaseName("UQ_RolePermissions_RoleId_PermissionId");
        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        //Description
        builder.Property(p=>p.AssignedAt)
            .HasColumnName("AssignedAt")
                .HasColumnType("timestamptz")
                .IsRequired()
                .HasDefaultValueSql("NOW()")
                .HasComment("تاریخ اتصال مجوز به نقش");
        // ═══════════════════════════════════════════════════════════════
        // Relations
        // ═══════════════════════════════════════════════════════════════
        builder.HasOne<Role>()
            .WithMany(r=>r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Permission>()
            .WithMany()
            .HasForeignKey(rp => rp.PermissionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
