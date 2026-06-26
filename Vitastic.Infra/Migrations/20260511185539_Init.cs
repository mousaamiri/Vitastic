using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vitastic.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 1, comment: "Order for displaying categories in UI"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Whether category is visible to users"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Discount type: Percentage or FixedAmount"),
                    Scope = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Discount scope: Global, SpecificCourses, SpecificCategories, SpecificInstructors"),
                    PercentageValue = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m, comment: "Percentage value (0-100)"),
                    FixedAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true, defaultValue: "IRT"),
                    FixedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MinimumOrderCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true, defaultValue: "IRT"),
                    MinimumOrderAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    MaximumDiscountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: true, defaultValue: "IRT"),
                    MaximumDiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    UsageLimit = table.Column<int>(type: "integer", nullable: true, comment: "Maximum number of times this discount can be used (null = unlimited)"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsSingleUse = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "If true, each user can use this discount only once"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Content = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    OccurredOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Error = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UsageCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0, comment: "Number of times this tag has been used"),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true, comment: "Whether tag is visible and usable"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ActiveCode = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ActiveCodeExpiresAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ActiveCode_Value = table.Column<string>(type: "text", nullable: true),
                    ResetPasswordToken = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ResetPasswordExpiresAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ResetPasswordToken_Value = table.Column<string>(type: "text", nullable: true),
                    UserAvatarFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SecurityStamp = table.Column<Guid>(type: "uuid", maxLength: 36, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    RegisterDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    LastLoginDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCategories",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCategories", x => new { x.DiscountId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_DiscountCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountCategories_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()", comment: "تاریخ اتصال مجوز به نقش"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    SessionId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    UserFullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    UserEmail = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.CheckConstraint("CK_Carts_UserOrSession", "(\"UserId\" IS NOT NULL AND \"SessionId\" IS NULL) OR (\"UserId\" IS NULL AND \"SessionId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountUsages",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountUsages", x => new { x.DiscountId, x.UserId });
                    table.ForeignKey(
                        name: "FK_DiscountUsages_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountUsages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instructors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Avatar = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Expertise = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false, comment: "Instructor status: Active, Inactive, PendingApproval, Suspended, Rejected"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instructors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instructors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ItemsTotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ShippingAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FinalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Order status: Pending, Processing, Completed, Cancelled, Refunded"),
                    PaymentMethod = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Payment method: Gateway, Wallet"),
                    CompletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    CancelledAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    PaymentTransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    PaymentGateway = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: true),
                    DiscountCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BillingStreet = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BillingCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BillingState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BillingCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BillingPostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BillingAdditionalInfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BillingAddress_Value = table.Column<string>(type: "text", nullable: true),
                    ShippingStreet = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ShippingCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ShippingState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ShippingCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ShippingPostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ShippingAdditionalInfo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShippingAddress_Value = table.Column<string>(type: "text", nullable: true),
                    CustomerNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AdminNote = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()", comment: "تاریخ اتصال نقش به کاربر"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "IRT"),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    ShortDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Slug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ThumbnailName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DemoVideoName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Course status: Draft, Published, Archived"),
                    Level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Course level: Beginner, Intermediate, Advanced, Expert"),
                    HasCertificate = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscountInstructors",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountInstructors", x => new { x.DiscountId, x.InstructorId });
                    table.ForeignKey(
                        name: "FK_DiscountInstructors_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountInstructors_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstructorRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstructorRatings_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstructorRatings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstructorSkills",
                columns: table => new
                {
                    Skill = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    InstructorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstructorSkills", x => new { x.InstructorId, x.Skill });
                    table.ForeignKey(
                        name: "FK_InstructorSkills_Instructors_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "Instructors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", maxLength: 20, nullable: false, defaultValue: 2),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderNotes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount_Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "IRT", comment: "Currency code (IRT, USD, etc.)"),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Transaction amount (can be negative for withdrawals)"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "User-provided transaction description"),
                    TransactionDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()", comment: "When transaction was created"),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Transaction status: Pending, Completed, Failed, Canceled, Reverted"),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "Transaction Type:  Deposit, Withdraw"),
                    CompletedDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "When transaction was completed/failed/canceled"),
                    RevertedDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "When transaction was reverted"),
                    PaymentAuthority = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Gateway transaction authority code"),
                    PaymentRefId = table.Column<int>(type: "integer", nullable: false, comment: "Gateway reference ID (0 for pending)"),
                    PaymentGateway = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "Payment gateway name (Zarinpal, Payping, etc.)"),
                    PaymentPaidDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "Date when payment was completed"),
                    PaymentDescription = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Additional payment notes from gateway"),
                    PaymentInfo_Value = table.Column<string>(type: "text", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CartId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, comment: "Snapshot of course title when added to cart"),
                    CourseInstructorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false, comment: "Snapshot of Course Instructor Name when added to cart"),
                    CourseImageName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true, comment: "Snapshot of course image when added to cart"),
                    Currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, comment: "Currency code (e.g., IRT, IRR, USD)"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "Snapshot of course price when added to cart"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()", comment: "تاریخ اتصال دوره به دسته بندی"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseCategories_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<decimal>(type: "numeric(2,1)", nullable: false),
                    Comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseRatings_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseRatings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()", comment: "تاریخ اتصال دوره به برچسب"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseTags_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCourses",
                columns: table => new
                {
                    DiscountId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCourses", x => new { x.DiscountId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_DiscountCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountCourses_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ImageName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    InstructorFullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UnitPriceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "IRT"),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DiscountAmountCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "IRT"),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    FinalPriceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "IRT"),
                    FinalPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    AccessExpiryDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    IsAccessGranted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AccessGrantedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    AccessRevokedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, comment: "Order of section within course"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    VideoFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: false, comment: "Episode duration in ticks (TimeSpan)"),
                    PriceCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "IRT"),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m, comment: "Episode price (0 = free)"),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false, comment: "Order of episode within section"),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false, comment: "شناسه کاربر ایجادکننده"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false, comment: "تاریخ ایجاد"),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر بروزرسان"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ آخرین بروزرسانی"),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false, comment: "وضعیت حذف"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true, comment: "تاریخ حذف"),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true, comment: "شناسه کاربر حذف‌کننده")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episodes_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CreatedAt",
                table: "CartItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_IsDeleted",
                table: "CartItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_IsDeleted_CreatedAt",
                table: "CartItems",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId",
                table: "CartItems",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_CourseId_Unique",
                table: "CartItems",
                columns: new[] { "CartId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CourseId",
                table: "CartItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_CreatedAt",
                table: "Carts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_IsDeleted",
                table: "Carts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_IsDeleted_CreatedAt",
                table: "Carts",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_IsDeleted_SessionId_CreatedAt",
                table: "Carts",
                columns: new[] { "IsDeleted", "SessionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_IsDeleted_UserId",
                table: "Carts",
                columns: new[] { "IsDeleted", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_SessionId",
                table: "Carts",
                column: "SessionId",
                filter: "\"SessionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true,
                filter: "\"UserId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsActive_DisplayOrder",
                table: "Categories",
                columns: new[] { "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted_IsActive_DisplayOrder",
                table: "Categories",
                columns: new[] { "IsDeleted", "IsActive", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name_Unique",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                filter: "\"ParentCategoryId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId_DisplayOrder",
                table: "Categories",
                columns: new[] { "ParentCategoryId", "DisplayOrder" },
                filter: "\"ParentCategoryId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_CreatedAt",
                table: "Categories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted",
                table: "Categories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Category_IsDeleted_CreatedAt",
                table: "Categories",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategories_CategoryId",
                table: "CourseCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategory_CreatedAt",
                table: "CourseCategories",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "UQ_CourseCategories_CourseId_CategoryId",
                table: "CourseCategories",
                columns: new[] { "CourseId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseRatings_CourseId_UserId",
                table: "CourseRatings",
                columns: new[] { "CourseId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseRatings_UserId",
                table: "CourseRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_CreatedAt",
                table: "Courses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted",
                table: "Courses",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Course_IsDeleted_CreatedAt",
                table: "Courses",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstructorId",
                table: "Courses",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsDeleted_Status_CreatedAt",
                table: "Courses",
                columns: new[] { "IsDeleted", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsPublished_Status",
                table: "Courses",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Slug",
                table: "Courses",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Title",
                table: "Courses",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTag_CreatedAt",
                table: "CourseTags",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTags_TagId",
                table: "CourseTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "UQ_CourseTags_CourseId_TagId",
                table: "CourseTags",
                columns: new[] { "CourseId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCategories_CategoryId",
                table: "DiscountCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCourses_CourseId",
                table: "DiscountCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountInstructors_InstructorId",
                table: "DiscountInstructors",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_CreatedAt",
                table: "Discounts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_IsDeleted",
                table: "Discounts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Discount_IsDeleted_CreatedAt",
                table: "Discounts",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Code",
                table: "Discounts",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_IsActive_Dates",
                table: "Discounts",
                columns: new[] { "IsActive", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_Scope_IsActive",
                table: "Discounts",
                columns: new[] { "Scope", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_StartDate_EndDate",
                table: "Discounts",
                columns: new[] { "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountUsages_UserId",
                table: "DiscountUsages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Episode_CreatedAt",
                table: "Episodes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Episode_IsDeleted",
                table: "Episodes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Episode_IsDeleted_CreatedAt",
                table: "Episodes",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_Price",
                table: "Episodes",
                column: "Price",
                filter: "\"Price\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SectionId",
                table: "Episodes",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_SectionId_DisplayOrder",
                table: "Episodes",
                columns: new[] { "SectionId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_InstructorRatings_InstructorId_UserId",
                table: "InstructorRatings",
                columns: new[] { "InstructorId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorRatings_UserId",
                table: "InstructorRatings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_CreatedAt",
                table: "Instructors",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_IsDeleted",
                table: "Instructors",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_IsDeleted_CreatedAt",
                table: "Instructors",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_Status",
                table: "Instructors",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Instructors_UserId",
                table: "Instructors",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorSkills_Value",
                table: "InstructorSkills",
                column: "Skill");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_CreatedAt",
                table: "OrderItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_IsDeleted",
                table: "OrderItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_IsDeleted_CreatedAt",
                table: "OrderItems",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_CourseId_IsDeleted",
                table: "OrderItems",
                columns: new[] { "CourseId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_IsAccessGranted_AccessExpiryDate",
                table: "OrderItems",
                columns: new[] { "IsAccessGranted", "AccessExpiryDate" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_CourseId",
                table: "OrderItems",
                columns: new[] { "OrderId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderNote_CreatedAt",
                table: "OrderNotes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_OrderNote_IsDeleted",
                table: "OrderNotes",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_OrderNote_IsDeleted_CreatedAt",
                table: "OrderNotes",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderNotes_OrderId",
                table: "OrderNotes",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedAt",
                table: "Orders",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IsDeleted",
                table: "Orders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Order_IsDeleted_CreatedAt",
                table: "Orders",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DiscountId",
                table: "Orders",
                column: "DiscountId",
                filter: "\"DiscountId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsDeleted_Status_CreatedAt",
                table: "Orders",
                columns: new[] { "IsDeleted", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PaymentTransactionId",
                table: "Orders",
                column: "PaymentTransactionId",
                filter: "\"PaymentTransactionId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_CreatedAt",
                table: "Orders",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId_CreatedAt",
                table: "Orders",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_CreatedAt",
                table: "PaymentTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_IsDeleted",
                table: "PaymentTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransaction_IsDeleted_CreatedAt",
                table: "PaymentTransactions",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Amount",
                table: "PaymentTransactions",
                column: "Amount");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_IsDeleted_Status_TransactionDate",
                table: "PaymentTransactions",
                columns: new[] { "IsDeleted", "Status", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId",
                table: "PaymentTransactions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_OrderId_Status",
                table: "PaymentTransactions",
                columns: new[] { "OrderId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_PaymentInfo_Authority",
                table: "PaymentTransactions",
                column: "PaymentAuthority");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Status_TransactionDate",
                table: "PaymentTransactions",
                columns: new[] { "Status", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_TransactionDate",
                table: "PaymentTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_Type_TransactionDate",
                table: "PaymentTransactions",
                columns: new[] { "Type", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_WalletId",
                table: "PaymentTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTransactions_WalletId_TransactionDate",
                table: "PaymentTransactions",
                columns: new[] { "WalletId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_CreatedAt",
                table: "Permissions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_IsDeleted",
                table: "Permissions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_IsDeleted_CreatedAt",
                table: "Permissions",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_CreatedAt",
                table: "RolePermissions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "UQ_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreatedAt",
                table: "Roles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Role_IsDeleted",
                table: "Roles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Role_IsDeleted_CreatedAt",
                table: "Roles",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Name",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Section_CreatedAt",
                table: "Sections",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Section_IsDeleted",
                table: "Sections",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Section_IsDeleted_CreatedAt",
                table: "Sections",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId",
                table: "Sections",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_DisplayOrder",
                table: "Sections",
                columns: new[] { "CourseId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseId_Title",
                table: "Sections",
                columns: new[] { "CourseId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Tag_CreatedAt",
                table: "Tags",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_IsDeleted",
                table: "Tags",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Tag_IsDeleted_CreatedAt",
                table: "Tags",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IsActive_UsageCount",
                table: "Tags",
                columns: new[] { "IsActive", "UsageCount" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_IsDeleted_IsActive_UsageCount",
                table: "Tags",
                columns: new[] { "IsDeleted", "IsActive", "UsageCount" },
                descending: new[] { false, false, true });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UsageCount",
                table: "Tags",
                column: "UsageCount",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_CreatedAt",
                table: "UserRoles",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserRoles_RoleId_UserId",
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_User_IsDeleted",
                table: "Users",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_User_IsDeleted_CreatedAt",
                table: "Users",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive",
                table: "Users",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsActive_RegisterDate",
                table: "Users",
                columns: new[] { "IsActive", "RegisterDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_CreatedAt",
                table: "Wallets",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_IsDeleted",
                table: "Wallets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_IsDeleted_CreatedAt",
                table: "Wallets",
                columns: new[] { "IsDeleted", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_Balance",
                table: "Wallets",
                column: "Balance");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_IsDeleted_UserId_Currency",
                table: "Wallets",
                columns: new[] { "IsDeleted", "UserId", "Currency" });

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_Currency",
                table: "Wallets",
                columns: new[] { "UserId", "Currency" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CourseCategories");

            migrationBuilder.DropTable(
                name: "CourseRatings");

            migrationBuilder.DropTable(
                name: "CourseTags");

            migrationBuilder.DropTable(
                name: "DiscountCategories");

            migrationBuilder.DropTable(
                name: "DiscountCourses");

            migrationBuilder.DropTable(
                name: "DiscountInstructors");

            migrationBuilder.DropTable(
                name: "DiscountUsages");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "InstructorRatings");

            migrationBuilder.DropTable(
                name: "InstructorSkills");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderNotes");

            migrationBuilder.DropTable(
                name: "OutboxMessages");

            migrationBuilder.DropTable(
                name: "PaymentTransactions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Instructors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
