using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Users;

public class UserConfiguration : AggregateRootConfiguration<User, UserId>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION - Apply common configurations for all entities (e.g., audit properties, soft delete, etc.)
        // ═══════════════════════════════════════════════════════════════
        base.Configure(builder);
        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════
        builder.ToTable("Users");
        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - UserId (Strongly-Typed ID)
        // ═══════════════════════════════════════════════════════════════
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value,
                value => UserId.CreateFrom(value).Value)
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Simple (Single Property)
        // ═══════════════════════════════════════════════════════════════
        //Email
        builder.Property(x => x.Email)
            .HasColumnName("Email")
            .HasMaxLength(Email.MaxLength)
            .HasConversion(email => email.Value, value => Email.Create(value).Value)
            .IsRequired();
        builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Users_Email");
        //Username
        builder.Property(x => x.UserName)
            .HasColumnName("UserName")
            .HasMaxLength(UserName.MaxLength)
            .HasConversion(userName => userName.Value, value => UserName.Create(value).Value)
            .IsRequired();
        builder.HasIndex(x => x.UserName).IsUnique().HasDatabaseName("IX_Users_UserName");
        //Firstname
        builder.Property(x => x.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(FirstName.MaxLength)
            .HasConversion(firstName => firstName != null ? firstName.Value : null,
                value => value != null ? FirstName.Create(value).Value : null)
            .IsRequired(false);
        //Phone number
        builder.Property(x => x.PhoneNumber)
            .HasColumnName("PhoneNumber")
            .HasMaxLength(PhoneNumber.MaxLength)
            .HasConversion(phoneNumber => phoneNumber != null ? phoneNumber.Value : null,
                value => value != null ? PhoneNumber.Create(value).Value : null)
            .IsRequired(false);
        //Lastname
        builder.Property(x => x.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(LastName.MaxLength)
            .HasConversion(lastName => lastName != null ? lastName.Value : null,
                value => value != null ? LastName.Create(value).Value : null)
            .IsRequired(false);
        //Password
        builder.Property(u => u.Password)
            .HasColumnName("PasswordHash")
            .HasMaxLength(Password.HashMaxLength)
            .HasConversion(
                password => password.Hash,
                hash => Password.CreateFromHash(hash).Value
            )
            .IsRequired();
        //Avatar file name
        builder.Property(u => u.UserAvatar)
            .HasColumnName("UserAvatarFileName")
            .HasMaxLength(UserAvatar.MaxFileNameLength)
            .HasConversion(
                avatar => avatar.FileName,
                value => string.IsNullOrWhiteSpace(value)
                    ? UserAvatar.Default()
                    : UserAvatar.Create(value).Value
            )
            .IsRequired();
        //SecurityStamp
        builder.Property(u => u.SecurityStamp)
            .HasColumnName("SecurityStamp")
            .HasMaxLength(SecurityStamp.Length)
            .HasConversion(
                stamp => stamp.Value,
                value => SecurityStamp.Create(value).Value
            )
            .IsRequired();
        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Complex (Owned Entities)
        // ═══════════════════════════════════════════════════════════════
        //Active code
        builder.OwnsOne(u => u.ActiveCode, activeCode =>
        {
            //Map Code property
            activeCode.Property(ac => ac.Code)
                .HasColumnName("ActiveCode")
                .HasMaxLength(ActiveCode.MaxLength)
                .IsRequired(false);
            //Map ExpiresAt property
            activeCode.Property(ac => ac.ExpiresAt)
                .HasColumnName("ActiveCodeExpiresAt")
                .HasColumnType("timestamptz");
            //Don't use is required for properties, because the entire ActiveCode can be null (user may not have an active code)
        });
        //Active code
        builder.OwnsOne(u => u.ResetPasswordToken, resetPassToken =>
        {
            //Map Code property
            resetPassToken.Property(ac => ac.Code)
                .HasColumnName("ResetPasswordToken")
                .HasMaxLength(ActiveCode.MaxLength)
                .IsRequired(false);
            //Map ExpiresAt property
            resetPassToken.Property(ac => ac.ExpiresAt)
                .HasColumnName("ResetPasswordExpiresAt")
                .HasColumnType("timestamptz");
        });
        // ═══════════════════════════════════════════════════════════════
        // PRIMITIVE PROPERTIES
        // ═══════════════════════════════════════════════════════════════
        //IsActive
        builder.Property(x => x.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();

        //RegistrationDate
        builder.Property(x => x.RegisterDate)
            .HasColumnName("RegisterDate")
            .HasColumnType("timestamptz")
            .IsRequired();
        //LastLoginDate
        builder.Property(x => x.LastLoginDate)
            .HasColumnName("LastLoginDate")
            .HasColumnType("timestamptz")
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // IGNORE PROPERTIES
        // ═══════════════════════════════════════════════════════════════
        builder.Ignore(u => u.UserFullName);
        // ═══════════════════════════════════════════════════════════════
        // RELATIONSHIPS
        // ═══════════════════════════════════════════════════════════════
        builder
            .Navigation(u => u.UserRoles)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        // ═══════════════════════════════════════════════════════════════
        // INDEXES FOR PERFORMANCE
        // ═══════════════════════════════════════════════════════════════
        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");
        // Composite index for common query: active users by registration date
        builder.HasIndex(u => new { u.IsActive, u.RegisterDate })
            .HasDatabaseName("IX_Users_IsActive_RegisterDate");
    }
}
