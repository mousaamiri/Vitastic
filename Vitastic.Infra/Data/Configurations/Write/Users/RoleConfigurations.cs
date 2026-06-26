using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Users;

public class RoleConfigurations : AggregateRootConfiguration<Role, RoleId>
{
    public override void Configure(EntityTypeBuilder<Role> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations for all entities (e.g., audit properties, soft delete, etc.)
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("Roles");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        //Key
        builder.Property(r => r.Id)
            .HasConversion(r => r.Value,
                value => RoleId.CreateFrom(value).Value);
        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Simple (Single Property)
        // ═══════════════════════════════════════════════════════════════
        builder.Property(r => r.Name)
            .HasColumnName("Name")
            .HasMaxLength(RoleName.MaxLenght)
            .HasConversion(name => name.Value, value => RoleName.Create(value).Value)
            .IsRequired();
        builder.HasIndex(r => r.Name).IsUnique().HasDatabaseName("IX_Roles_Name");
        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════
        builder
            .Navigation(u => u.RolePermissions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
