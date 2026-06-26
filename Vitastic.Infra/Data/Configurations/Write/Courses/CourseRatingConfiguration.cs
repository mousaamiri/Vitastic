using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Data.Configurations.Write.Courses;

public class CourseRatingConfiguration : IEntityTypeConfiguration<CourseRating>
{
    public void Configure(EntityTypeBuilder<CourseRating> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => CourseRatingId.CreateFrom(value).Value);
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

        builder.HasOne(x => x.Course)
            .WithMany(c => c.Ratings)
            .HasForeignKey(x => x.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.CourseId, x.UserId })
            .IsUnique();
    }
}
