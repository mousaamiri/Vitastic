using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Users;

public class PermissionsConfiguration:FullEntityConfiguration<Permission,PermissionId>
{
    public override void Configure(EntityTypeBuilder<Permission> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations for all entities (e.g., audit properties, soft delete, etc.)
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("Permissions");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        //Key
        builder.Property(p => p.Id)
            .HasConversion(id=>id.Value,value=>PermissionId.CreateFrom(value).Value);
        // ═══════════════════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════════════════
        //Coe
        builder.Property(p=>p.Code)
            .IsRequired()
            .HasMaxLength(Permission.MaxCodeLength)
            .IsUnicode(false);
        builder.HasIndex(p=>p.Code).IsUnique()
            .HasDatabaseName("IX_Permissions_Code");
        //Description
        builder.Property(p=>p.Description).IsRequired().HasMaxLength(Permission.MaxDescriptionLength);
    }
}
