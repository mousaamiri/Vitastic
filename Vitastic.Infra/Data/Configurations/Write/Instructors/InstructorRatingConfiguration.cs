using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Data.Configurations.Write.Instructors;

public class InstructorRatingConfiguration : IEntityTypeConfiguration<InstructorRating>
{
    public void Configure(EntityTypeBuilder<InstructorRating> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => InstructorRatingId.CreateFrom(value).Value);
        builder.OwnsOne(x => x.Rating, rating =>
        {
            rating.Property(r => r.Value)
                .HasColumnName("Rating")
                .HasColumnType("decimal(2,1)")
                .IsRequired();

            rating.Property(r => r.Comment)
                .HasColumnName("Comment")
                .HasMaxLength(Rating.MaxCommentLength);
        });

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasOne(x => x.Instructor)
            .WithMany(i => i.Ratings)
            .HasForeignKey(x => x.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.InstructorId, x.UserId })
            .IsUnique();
    }
}
