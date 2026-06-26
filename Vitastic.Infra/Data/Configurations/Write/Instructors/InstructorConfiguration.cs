using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;


namespace Vitastic.Infra.Data.Configurations.Write.Instructors;

public sealed class InstructorConfiguration : AggregateRootConfiguration<Instructor, InstructorId>
{
    public override void Configure(EntityTypeBuilder<Instructor> builder)
    {
        #region Base Configuration

        base.Configure(builder);

        builder.ToTable("Instructors");

        #endregion

        #region Primary Key — InstructorId

        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => InstructorId.CreateFrom(value).Value)
            .IsRequired();

        #endregion

        #region Foreign Key — UserId (One-to-One with User)

        builder.Property(i => i.UserId)
            .HasColumnName("UserId")
            .HasConversion(
                id => id.Value,
                value => UserId.CreateFrom(value).Value)
            .IsRequired();

        // One instructor per user
        builder.HasIndex(i => i.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Instructors_UserId");

        // One-to-One relationship: Instructor → User
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<Instructor>(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        #endregion

        #region Value Object — InstructorBio

        builder.Property(i => i.Bio)
            .HasColumnName("Bio")
            .HasMaxLength(InstructorBio.MaxLength)
            .HasConversion(
                bio => bio.Value,
                value => InstructorBio.Create(value).Value)
            .IsRequired();

        #endregion

        #region Value Object — InstructorExpertise

        builder.Property(i => i.Expertise)
            .HasColumnName("Expertise")
            .HasMaxLength(InstructorExpertise.MaxLength)
            .HasConversion(
                expertise => expertise.Value,
                value => InstructorExpertise.Create(value).Value)
            .IsRequired();

        #endregion

        #region Enum — InstructorStatus

        builder.Property(i => i.Status)
            .HasColumnName("Status")
            .HasMaxLength(30)
            .HasConversion<string>()
            .IsRequired()
            .HasComment("Instructor status: Active, Inactive, PendingApproval, Suspended, Rejected");

        #endregion

        #region Value Object — InstructorSkills (Owned Collection)

        builder.OwnsOne(i => i.Skills, skillsBuilder =>
        {
            skillsBuilder.OwnsMany(s => s.Values, skillBuilder =>
            {
                skillBuilder.ToTable("InstructorSkills");

                // Composite key: InstructorId + Skill value
                skillBuilder.WithOwner()
                    .HasForeignKey("InstructorId");

                skillBuilder.Property(s => s.Value)
                    .HasColumnName("Skill")
                    .HasMaxLength(InstructorSkill.MaxLength)
                    .IsRequired();

                skillBuilder.HasKey("InstructorId", "Value");

                // Index for skill-based searches
                skillBuilder.HasIndex("Value")
                    .HasDatabaseName("IX_InstructorSkills_Value");
            });
        });

        #endregion

        #region Navigation — Ratings (Backing Field)

        // EF Core accesses the private _ratings field directly
        builder.HasMany(i => i.Ratings)
            .WithOne(r => r.Instructor)
            .HasForeignKey(r => r.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Tell EF to use the backing field for the collection
        builder.Navigation(i => i.Ratings)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        #endregion
        #region Value Object — FullName


        builder.Property(i => i.FullName )
            .HasColumnName("FullName")
            .HasMaxLength(FullName.MaxLength)
            .HasConversion(
                avatar => avatar.Value,
                value => FullName.Create(value).Value)
            .IsRequired(false);

        #endregion

        #region Value Object — Avatar

        builder.Property(i => i.Avatar)
            .HasColumnName("Avatar")
            .HasMaxLength(UserAvatar.MaxFileNameLength)
            .HasConversion(
                avatar => avatar.Value,
                value => UserAvatar.Create(value).Value)
            .IsRequired(false);

        #endregion

        #region Ignored Properties — Computed / Derived from User

        // Computed properties — calculated from Ratings collection in memory
        builder.Ignore(i => i.AverageRating);
        builder.Ignore(i => i.TotalRatings);

        #endregion

        #region Indexes

        // Composite index for filtering non-deleted instructors by creation date
        builder.HasIndex(i => new { i.IsDeleted, i.CreatedAt })
            .HasDatabaseName("IX_Instructors_IsDeleted_CreatedAt");

        // Index for filtering by status
        builder.HasIndex(i => i.Status)
            .HasDatabaseName("IX_Instructors_Status");

        #endregion
    }
}
