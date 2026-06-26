using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects.Base;
using Vitastic.Infra.Data.Configurations.Seed;
using Vitastic.Infra.Outbox;

namespace Vitastic.Infra.Data;

public class ApplicationWriteDbContext(DbContextOptions<ApplicationWriteDbContext> dbContextOptions)
    : DbContext(dbContextOptions)
{
    internal DbSet<User> Users => Set<User>();
    internal DbSet<Order> Orders => Set<Order>();
    internal DbSet<OrderItem> OrderItems => Set<OrderItem>();
    internal DbSet<OrderNote> OrderNotes => Set<OrderNote>();
    internal DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<Cart> Carts => Set<Cart>();
    internal DbSet<CartItem>  CartItems => Set<CartItem>();
    internal DbSet<Course> Courses => Set<Course>();
    internal DbSet<Section> Sections => Set<Section>();
    internal DbSet<Episode> Episodes => Set<Episode>();
    internal DbSet<Category> Categories => Set<Category>();
    internal DbSet<Role> Roles => Set<Role>();
    internal DbSet<Permission>  Permissions => Set<Permission>();

    internal DbSet<Instructor> Instructors => Set<Instructor>();
    internal DbSet<Tag> Tags => Set<Tag>();
    internal DbSet<Wallet> Wallets => Set<Wallet>();
    internal DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    //Associative Entity
    internal DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    internal DbSet<CourseRating> CourseRatings => Set<CourseRating>();
    internal DbSet<InstructorRating> InstructorRatings => Set<InstructorRating>();
    internal DbSet<UserRole> UserRoles => Set<UserRole>();
    internal DbSet<CourseTag>  CourseTags => Set<CourseTag>();
    internal DbSet<CourseCategory>  CourseCategories => Set<CourseCategory>();
    //Outbox pattern
    internal DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql();
        optionsBuilder.UseExceptionProcessor();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationWriteDbContext).Assembly,
            WriteConfigurationsFilter);

        //Ignore DomainEvents in all entities
        modelBuilder.Ignore<DomainEvent>();
        modelBuilder.Ignore(typeof(ValueObject<>));
        modelBuilder.Ignore(typeof(ValueObject<decimal>));
        modelBuilder.Ignore<ValueObject>();
        modelBuilder.Ignore<ValueObject<decimal>>();
        modelBuilder.Ignore<ValueObject<string>>();
        modelBuilder.Ignore<ValueObject<Guid>>();
        modelBuilder.Ignore(typeof(StronglyTypedId<,>));
        modelBuilder.Ignore(typeof(GuidBasedId<>));
        modelBuilder.Ignore(typeof(IntBasedId<>));

        modelBuilder.Ignore<DiscountCode>();
        modelBuilder.Ignore<PhoneNumber>();
        modelBuilder.Ignore<Money>();
        modelBuilder.Ignore<InstructorExpertise>();

        modelBuilder.Ignore<UserId>();
        modelBuilder.Ignore<CourseId>();
        modelBuilder.Ignore<CategoryId>();
        modelBuilder.Ignore<RoleId>();
        modelBuilder.Ignore<InstructorId>();
        modelBuilder.Ignore<TagId>();
        modelBuilder.Ignore<WalletId>();
        modelBuilder.Ignore<CartId>();
        modelBuilder.Ignore<CartItemId>();
        modelBuilder.Ignore<OrderId>();
        modelBuilder.Ignore<OrderItemId>();
        modelBuilder.Ignore<PaymentTransactionId>();
        modelBuilder.Ignore<PermissionId>();
        modelBuilder.Ignore<UserRoleId>();
        modelBuilder.Ignore<RolePermissionId>();
        modelBuilder.Ignore<InstructorRatingId>();
        modelBuilder.Ignore<CourseRatingId>();

    }

    private static bool WriteConfigurationsFilter(Type type)
        => type.FullName?.Contains("Configurations.Write", StringComparison.Ordinal) ?? false;
}
