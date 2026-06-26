using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Infra.Data.Configurations.Write.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Users;


public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(RefreshToken.TokenMaxLength);

        builder.Property(x => x.ExpiresAt).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.RevokedAt);
        builder.Property(x => x.ReplacedByToken).HasMaxLength(RefreshToken.TokenMaxLength);

        builder.HasIndex(x => x.Token).IsUnique();

        builder.Property(x => x.UserId)
            .HasConversion(userId => userId.Value, value => UserId.CreateFrom(value).Value);

        builder.HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
