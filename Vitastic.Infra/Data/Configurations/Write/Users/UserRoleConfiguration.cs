using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Users;

public class UserRolesConfiguration:BaseEntityConfiguration<UserRole,UserRoleId>
{
    public override void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("UserRoles");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        //Key
        builder.Property(p => p.Id)
            .HasConversion(id=>id.Value,value=>UserRoleId.CreateFrom(value).Value);
        // ═══════════════════════════════════════════════════════════════
        // ForeignKeys
        // ═══════════════════════════════════════════════════════════════

        builder.Property(p=>p.RoleId)
            .HasConversion(id => id.Value,value=>RoleId.CreateFrom(value).Value);
        builder.Property(p=>p.UserId)
            .HasConversion(id => id.Value,value=>UserId.CreateFrom(value).Value);
        builder.HasIndex(p=>new{p.RoleId,p.UserId})
            .IsUnique()
            .HasDatabaseName("UQ_UserRoles_RoleId_UserId");
        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════

        //Description
        builder.Property(p=>p.AssignedAt)
            .HasColumnName("AssignedAt")
                .HasColumnType("timestamptz")
                .IsRequired()
                .HasDefaultValueSql("NOW()")
                .HasComment("تاریخ اتصال نقش به کاربر");
        // ═══════════════════════════════════════════════════════════════
        // Relations
        // ═══════════════════════════════════════════════════════════════
        builder.HasOne<User>()
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Role>()
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
