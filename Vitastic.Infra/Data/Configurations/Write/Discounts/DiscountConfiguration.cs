using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.Enums;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data.Configurations.Base;

namespace Vitastic.Infra.Data.Configurations.Write.Discounts;

public sealed class DiscountConfiguration : AggregateRootConfiguration<Discount, DiscountId>
{
    public override void Configure(EntityTypeBuilder<Discount> builder)
    {
        // ═══════════════════════════════════════════════════════════════
        // BASE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        base.Configure(builder);

        // ═══════════════════════════════════════════════════════════════
        // TABLE CONFIGURATION
        // ═══════════════════════════════════════════════════════════════

        builder.ToTable("Discounts");

        // ═══════════════════════════════════════════════════════════════
        // PRIMARY KEY - DiscountId
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.Id)
            .HasColumnName("Id")
            .HasConversion(
                id => id.Value,
                value => DiscountId.CreateFrom(value).Value
            )
            .IsRequired();

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECT - DiscountCode (Unique Business Key)
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.Code)
            .HasColumnName("Code")
            .HasMaxLength(DiscountCode.MaxLength)
            .HasConversion(
                code => code.Value,
                value => DiscountCode.Create(value).Value
            )
            .IsRequired();

        // Unique index on discount code
        builder.HasIndex(d => d.Code)
            .IsUnique()
            .HasDatabaseName("IX_Discounts_Code");

        // ═══════════════════════════════════════════════════════════════
        // VALUE OBJECTS - Title & Description
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.Title)
            .HasColumnName("Title")
            .HasMaxLength(Title.MaxLength)
            .HasConversion(
                title => title.Value,
                value => Title.Create(value).Value
            )
            .IsRequired();

        builder.Property(d => d.Description)
            .HasColumnName("Description")
            .HasMaxLength(Description.MaxLength)
            .HasConversion(
                desc => desc != null ? desc.Value : null,
                value => value != null ? Description.Create(value).Value : null
            )
            .IsRequired(false);

        // ═══════════════════════════════════════════════════════════════
        // ENUMS - Type & Scope
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.Type)
            .HasColumnName("Type")
            .HasMaxLength(20)
            .HasConversion(v=>v.ToString(),
                v=>(DiscountType)Enum.Parse(typeof(DiscountType),v))
            .IsRequired()
            .HasComment("Discount type: Percentage or FixedAmount");

        builder.Property(d => d.Scope)
            .HasColumnName("Scope")
            .HasMaxLength(30)
            .HasConversion(v=>v.ToString(),
                v=>(DiscountScope)Enum.Parse(typeof(DiscountScope),v))
            .IsRequired()
            .HasComment("Discount scope: Global, SpecificCourses, SpecificCategories, SpecificInstructors");

        // ═══════════════════════════════════════════════════════════════
        // DISCOUNT VALUES
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.PercentageValue)
            .HasColumnName("PercentageValue")
            .HasColumnType("decimal(5,2)")
            .IsRequired()
            .HasDefaultValue(0)
            .HasComment("Percentage value (0-100)");

        builder.OwnsOne(d => d.FixedAmountValue, fixedAmount =>
        {

            fixedAmount.Property(m => m.Value)
                .HasColumnName("FixedAmount")
                .HasColumnType("decimal(18,2)");
            //Don't use "IsRequired" here because we want to allow null for non-fixed discounts

            fixedAmount.Property(m => m.Currency)
                .HasColumnName("FixedAmountCurrency")
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                ).HasDefaultValue(Currency.IranianToman);
            //Don't use "IsRequired" here because we want to allow null for non-fixed discounts
        });



        // ═══════════════════════════════════════════════════════════════
        // CONSTRAINTS - Min/Max Amounts
        // ═══════════════════════════════════════════════════════════════

        builder.OwnsOne<Money>(d => d.MinimumOrderAmount, minAmount =>
        {
            minAmount.Property(m => m.Value)
                .HasColumnName("MinimumOrderAmount")
                .HasColumnType("decimal(18,2)");
            //Don't use "IsRequired" here because we want to allow null for no minimum

            minAmount.Property(m => m.Currency)
                .HasColumnName("MinimumOrderCurrency")
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                ).HasDefaultValue(Currency.IranianToman);
            //Don't use "IsRequired" here because we want to allow null for no minimum
        });

        builder.OwnsOne(d => d.MaximumDiscountAmount, maxAmount =>
        {
            maxAmount.Property(m => m.Value)
                .HasColumnName("MaximumDiscountAmount")
                .HasColumnType("decimal(18,2)");
            //Don't use "IsRequired" here because we want to allow null for no maximum

            maxAmount.Property(m => m.Currency)
                .HasColumnName("MaximumDiscountCurrency")
                .HasMaxLength(Currency.CodeLength)
                .HasConversion(
                    currency => currency.Code,
                    code => Currency.FromCode(code).Value
                ).HasDefaultValue(Currency.IranianToman);
            //Don't use "IsRequired" here because we want to allow null for no maximum
        });

        // ═══════════════════════════════════════════════════════════════
        // DATE RANGE
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.StartDate)
            .HasColumnName("StartDate")
            .HasColumnType("timestamptz")
            .IsRequired();

        builder.Property(d => d.EndDate)
            .HasColumnName("EndDate")
            .HasColumnType("timestamptz")
            .IsRequired();

        // Index for active discount queries
        builder.HasIndex(d => new { d.StartDate, d.EndDate })
            .HasDatabaseName("IX_Discounts_StartDate_EndDate");

        // ═══════════════════════════════════════════════════════════════
        // USAGE TRACKING
        // ═══════════════════════════════════════════════════════════════

        builder.Property(d => d.UsageLimit)
            .HasColumnName("UsageLimit")
            .IsRequired(false)
            .HasComment("Maximum number of times this discount can be used (null = unlimited)");

        builder.Property(d => d.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.IsSingleUse)
            .HasColumnName("IsSingleUse")
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("If true, each user can use this discount only once");

        // ═══════════════════════════════════════════════════════════════
        // USED BY USERS (Many-to-Many Junction Table)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Tracks which users have used this discount
        /// Used for single-use validation and analytics
        /// </summary>
        builder.HasMany<User>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "DiscountUsages",
                j => j.HasOne<User>()
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Discount>()
                    .WithMany()
                    .HasForeignKey("DiscountId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("DiscountUsages");
                    j.HasKey("DiscountId", "UserId");

                    // Track when user used the discount
                    j.Property<DateTime>("UsedAt")
                        .HasColumnName("UsedAt")
                        .HasColumnType("timestamptz")
                        .HasDefaultValueSql("NOW()");

                    j.HasIndex("UserId")
                        .HasDatabaseName("IX_DiscountUsages_UserId");
                }
            );

        // Ignore UsedByUserIds (data comes from junction table)
        builder.Ignore(d => d.UsedByUserIds);

        // ═══════════════════════════════════════════════════════════════
        // SCOPE TARGETS - Courses (Many-to-Many)
        // ═══════════════════════════════════════════════════════════════

        builder.HasMany<Course>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "DiscountCourses",
                j => j.HasOne<Course>()
                    .WithMany()
                    .HasForeignKey("CourseId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Discount>()
                    .WithMany()
                    .HasForeignKey("DiscountId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("DiscountCourses");
                    j.HasKey("DiscountId", "CourseId");

                    j.HasIndex("CourseId")
                        .HasDatabaseName("IX_DiscountCourses_CourseId");
                }
            );

        // Ignore ApplicableCourseIds
        builder.Ignore(d => d.ApplicableCourseIds);

        // ═══════════════════════════════════════════════════════════════
        // SCOPE TARGETS - Categories (Many-to-Many)
        // ═══════════════════════════════════════════════════════════════

        builder.HasMany<Category>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "DiscountCategories",
                j => j.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey("CategoryId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Discount>()
                    .WithMany()
                    .HasForeignKey("DiscountId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("DiscountCategories");
                    j.HasKey("DiscountId", "CategoryId");

                    j.HasIndex("CategoryId")
                        .HasDatabaseName("IX_DiscountCategories_CategoryId");
                }
            );

        // Ignore ApplicableCategoryIds
        builder.Ignore(d => d.ApplicableCategoryIds);

        // ═══════════════════════════════════════════════════════════════
        // SCOPE TARGETS - Instructors (Many-to-Many)
        // ═══════════════════════════════════════════════════════════════

        builder.HasMany<Instructor>()
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "DiscountInstructors",
                j => j.HasOne<Instructor>()
                    .WithMany()
                    .HasForeignKey("InstructorId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Discount>()
                    .WithMany()
                    .HasForeignKey("DiscountId")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("DiscountInstructors");
                    j.HasKey("DiscountId", "InstructorId");

                    j.HasIndex("InstructorId")
                        .HasDatabaseName("IX_DiscountInstructors_InstructorId");
                }
            );

        // Ignore ApplicableInstructorIds
        builder.Ignore(d => d.ApplicableInstructorIds);

        // ═══════════════════════════════════════════════════════════════
        // ADDITIONAL INDEXES
        // ═══════════════════════════════════════════════════════════════

        // Composite index for finding active discounts
        builder.HasIndex(d => new { d.IsActive, d.StartDate, d.EndDate })
            .HasDatabaseName("IX_Discounts_IsActive_Dates");

        // Index for scope-based queries
        builder.HasIndex(d => new { d.Scope, d.IsActive })
            .HasDatabaseName("IX_Discounts_Scope_IsActive");
    }
}
