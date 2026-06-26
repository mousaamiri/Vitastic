using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Categories;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.Enums;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Tags;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Infra.Data.Configurations.Seed;

public class DatabaseSeeder(ApplicationWriteDbContext context)
{
    public async Task SeedAsync()
    {
        await SeedCategoriesAsync();
        await SeedTagsAsync();
        await SeedUsersAsync();
        await SeedInstructorsAsync();
        await SeedWalletsAsync();
        await SeedTransactionsAsync();
        await SeedPermissionsAsync();
        await SeedRolesAsync();
        await SeedUserRolesAsync();
        await SeedDiscountsAsync();
        await SeedCoursesAsync();
        await SeedCourseRatingsAsync();
        await SeedInstructorRatingsAsync();
        await SeedOrdersAsync();
    }

    private async Task SeedCategoriesAsync()
    {
        if (await context.Categories.AnyAsync())
            return;

        // Main categories
        var programming = Category.Create("برنامه‌نویسی", "programming", 1, null).Value;
        var design = Category.Create("طراحی", "design", 2, null).Value;
        var business = Category.Create("کسب و کار", "business", 3, null).Value;
        var marketing = Category.Create("بازاریابی دیجیتال", "digital-marketing", 4, null).Value;
        var lifestyle = Category.Create("سبک زندگی", "lifestyle", 5, null).Value;
        var language = Category.Create("زبان", "language", 6, null).Value;

        context.Categories.AddRange(programming, design, business, marketing, lifestyle, language);
        await context.SaveChangesAsync();

        // Subcategories
        var subcategories = new[]
        {
            // Programming subcategories
            Category.Create("توسعه وب", "web-development", 1, programming.Id).Value,
            Category.Create("موبایل", "mobile-development", 2, programming.Id).Value,
            Category.Create("دیتابیس", "database", 3, programming.Id).Value,

            // Design subcategories
            Category.Create("طراحی UI/UX", "ui-ux-design", 1, design.Id).Value,
            Category.Create("گرافیک", "graphic-design", 2, design.Id).Value,
            Category.Create("موشن گرافیک", "motion-graphics", 3, design.Id).Value,

            // Business subcategories
            Category.Create("مدیریت پروژه", "project-management", 1, business.Id).Value,
            Category.Create("کارآفرینی", "entrepreneurship", 2, business.Id).Value,

            // Marketing subcategories
            Category.Create("سئو", "seo", 1, marketing.Id).Value,
            Category.Create("شبکه‌های اجتماعی", "social-media", 2, marketing.Id).Value,

            // Lifestyle subcategories
            Category.Create("تناسب اندام", "fitness", 1, lifestyle.Id).Value,
            Category.Create("آشپزی", "cooking", 2, lifestyle.Id).Value,

            // Language subcategories
            Category.Create("انگلیسی", "english", 1, language.Id).Value,
            Category.Create("آلمانی", "german", 2, language.Id).Value,
        };

        context.Categories.AddRange(subcategories);
        await context.SaveChangesAsync();
    }

    private async Task SeedTagsAsync()
    {
        if (await context.Tags.AnyAsync())
            return;

        var tags = new[]
        {
            Tag.Create("C#").Value,
            Tag.Create("ASP.NET Core").Value,
            Tag.Create("React").Value,
            Tag.Create("Vue.js").Value,
            Tag.Create("Angular").Value,
            Tag.Create("JavaScript").Value,
            Tag.Create("TypeScript").Value,
            Tag.Create("Python").Value,
            Tag.Create("Django").Value,
            Tag.Create("Flask").Value,
            Tag.Create("Flutter").Value,
            Tag.Create("React Native").Value,
            Tag.Create("Swift").Value,
            Tag.Create("Kotlin").Value,
            Tag.Create("Figma").Value,
            Tag.Create("Adobe XD").Value,
            Tag.Create("Photoshop").Value,
            Tag.Create("Illustrator").Value,
            Tag.Create("After Effects").Value,
            Tag.Create("Scrum").Value,
            Tag.Create("Agile").Value,
            Tag.Create("SEO").Value,
            Tag.Create("Google Ads").Value,
            Tag.Create("Social Media Marketing").Value,
        };

        context.Tags.AddRange(tags);
        await context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        if (await context.Users.AnyAsync())
            return;

        var users = new List<User>();

        // Admin user
        var admin = User.CreateByAdmin(
            UserName.Create("admin").Value,
            Email.Create("admin@vitastic.com").Value,
            Password.Create("1234Ma@").Value,
            FirstName.Create("مدیر").Value,
            LastName.Create("سیستم").Value,
            null,null,true
        ).Value;
        users.Add(admin);

        // Regular users
        var userData = new[]
        {
            ("reza_ahmadi", "reza.ahmadi@example.com", "رضا", "احمدی"),
            ("sara_mohammadi", "sara.mohammadi@example.com", "سارا", "محمدی"),
            ("ali_karimi", "ali.karimi@example.com", "علی", "کریمی"),
            ("maryam_hosseini", "maryam.hosseini@example.com", "مریم", "حسینی"),
            ("mehdi_rezaei", "mehdi.rezaei@example.com", "مهدی", "رضایی"),
            ("zahra_safari", "zahra.safari@example.com", "زهرا", "صفری"),
            ("hossein_moradi", "hossein.moradi@example.com", "حسین", "مرادی"),
            ("fatemeh_jafari", "fatemeh.jafari@example.com", "فاطمه", "جعفری"),
            ("mohammad_bagheri", "mohammad.bagheri@example.com", "محمد", "باقری"),
        };

        foreach (var (username, email, firstName, lastName) in userData)
        {
            var user = User.CreateByAdmin(
                UserName.Create(username).Value,
                Email.Create(email).Value,
                Password.Create("1234Ma@").Value,
                FirstName.Create(firstName).Value,
                LastName.Create(lastName).Value,
                null,null,true
            ).Value;
            users.Add(user);
        }

        context.Users.AddRange(users);
        await context.SaveChangesAsync();
    }

    private async Task SeedInstructorsAsync()
    {
        if (await context.Instructors.AnyAsync())
            return;

        var users = (await context.Users.ToListAsync())
            .Where(u => !string.Equals(u.UserName.Value, "admin", StringComparison.Ordinal))
            .Take(6)
            .ToList();

        var instructorData = new[]
        {
            ("متخصص توسعه وب با 8 سال تجربه در ASP.NET Core و React", "برنامه‌نویسی وب و توسعه Full Stack"),
            ("طراح UI/UX با بیش از 100 پروژه موفق و تجربه کار با شرکت‌های بزرگ", "طراحی رابط کاربری و تجربه کاربری"),
            ("توسعه‌دهنده موبایل Flutter با تجربه انتشار 20+ اپلیکیشن", "برنامه‌نویسی موبایل Cross-platform"),
            ("مدرس مدیریت پروژه PMP و مشاور کسب و کار", "مدیریت پروژه و Agile"),
            ("متخصص بازاریابی دیجیتال و سئو با 5 سال تجربه", "بازاریابی دیجیتال و تبلیغات آنلاین"),
            ("طراح گرافیک و موشن گرافیک با تخصص در Adobe Suite", "طراحی گرافیک و انیمیشن")
        };

        var instructors = new List<Instructor>();

        for (int i = 0; i < Math.Min(users.Count, instructorData.Length); i++)
        {
            var (bio, expertise) = instructorData[i];
            var user = users[i];
            var instructor = Instructor.Create(
                user.Id,
                user.UserFullName,
                user.UserAvatar,
                InstructorBio.Create(bio).Value,
                InstructorExpertise.Create(expertise).Value
            ).Value;

            instructor.Activate();
            instructors.Add(instructor);
        }

        context.Instructors.AddRange(instructors);
        await context.SaveChangesAsync();
    }

    private async Task SeedWalletsAsync()
    {
        if (await context.Wallets.AnyAsync())
            return;

        var users = await context.Users.ToListAsync();
        var wallets = new List<Wallet>();

        foreach (var user in users)
        {
            var wallet = Wallet.Create(user.Id, "IRT").Value;
            wallets.Add(wallet);
        }

        context.Wallets.AddRange(wallets);
        await context.SaveChangesAsync();
    }

    private async Task SeedTransactionsAsync()
    {
        if (await context.PaymentTransactions.AnyAsync())
            return;

        var wallets = await context.Wallets.ToListAsync();

        foreach (var wallet in wallets)
        {
            // 2-4 تراکنش تصادفی برای هر کیف پول
            var transactionCount = Random.Shared.Next(2, 5);

            for (int i = 0; i < transactionCount; i++)
            {
                var amount = Random.Shared.Next(100_000, 5_000_000);
                var depositResult = wallet.AddFunds(amount, $"واریز اولیه #{i + 1}");

                if (depositResult.IsSuccess)
                {
                    context.PaymentTransactions.Add(depositResult.Value);
                }
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedPermissionsAsync()
    {
        if (await context.Permissions.AnyAsync())
            return;

        var permissions = new[]
        {
            Permission.Create("USERS.VIEW", "مشاهده کاربران").Value,
            Permission.Create("USERS.CREATE", "ایجاد کاربر").Value,
            Permission.Create("USERS.EDIT", "ویرایش کاربر").Value,
            Permission.Create("USERS.DELETE", "حذف کاربر").Value,
            Permission.Create("COURSES.VIEW", "مشاهده دوره‌ها").Value,
            Permission.Create("COURSES.CREATE", "ایجاد دوره").Value,
            Permission.Create("COURSES.EDIT", "ویرایش دوره").Value,
            Permission.Create("COURSES.DELETE", "حذف دوره").Value,
            Permission.Create("ORDERS.VIEW", "مشاهده سفارشات").Value,
            Permission.Create("ORDERS.MANAGE", "مدیریت سفارشات").Value,
            Permission.Create("TRANSACTIONS.VIEW", "مشاهده تراکنش‌ها").Value,
            Permission.Create("SETTINGS.MANAGE", "مدیریت تنظیمات").Value
        };

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (await context.Roles.AnyAsync())
            return;

        var adminRole = Role.Create("Admin").Value;
        var instructorRole = Role.Create("Instructor").Value;
        var studentRole = Role.Create("Student").Value;

        context.Roles.AddRange(adminRole, instructorRole, studentRole);
        await context.SaveChangesAsync();

        // دسترسی‌ها رو به نقش‌ها اضافه می‌کنیم
        var permissions = await context.Permissions.ToListAsync();

        // Admin: همه دسترسی‌ها
        foreach (var permission in permissions)
        {
            var rolePermission = RolePermission.Create(adminRole.Id, permission.Id);
            context.RolePermissions.Add(rolePermission);
        }

        // Instructor: دسترسی‌های مربوط به دوره
        var instructorPermissions = permissions.Where(p =>
            p.Code.StartsWith("COURSES.") || string.Equals(p.Code, "ORDERS.VIEW", StringComparison.Ordinal)).ToList();
        foreach (var permission in instructorPermissions)
        {
            var rolePermission = RolePermission.Create(instructorRole.Id, permission.Id);
            context.RolePermissions.Add(rolePermission);
        }

        // Student: فقط مشاهده
        var studentPermissions = permissions.Where(p =>
            string.Equals(p.Code, "COURSES.VIEW", StringComparison.Ordinal)
            || string.Equals(p.Code, "ORDERS.VIEW", StringComparison.Ordinal)).ToList();
        foreach (var permission in studentPermissions)
        {
            var rolePermission = RolePermission.Create(studentRole.Id, permission.Id);
            context.RolePermissions.Add(rolePermission);
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedUserRolesAsync()
    {
        if (await context.UserRoles.AnyAsync())
            return;

        var users = await context.Users.ToListAsync();
        var roles = await context.Roles.ToListAsync();
        var adminRole = roles.First(r => string.Equals(r.Name.Value, "Admin", StringComparison.Ordinal));
        var instructorRole = roles.First(r => string.Equals(r.Name.Value, "Instructor", StringComparison.Ordinal));
        var studentRole = roles.First(r => string.Equals(r.Name.Value, "Student", StringComparison.Ordinal));

        foreach (var user in users)
        {
            if (string.Equals(user.UserName.Value, "admin", StringComparison.Ordinal))
            {
                context.UserRoles.Add(UserRole.Create(adminRole.Id, user.Id));
            }
            else
            {
                var isInstructor = await context.Instructors.AnyAsync(i => i.UserId.Equals(user.Id));
                var role = isInstructor ? instructorRole : studentRole;
                context.UserRoles.Add(UserRole.Create(role.Id, user.Id));
            }
        }

        await context.SaveChangesAsync();
    }

    private async Task SeedDiscountsAsync()
    {
        if (await context.Discounts.AnyAsync())
            return;

        var now = DateTimeOffset.UtcNow;
        var courses = await context.Courses.Take(3).ToListAsync();
        var categories = await context.Categories.Take(2).ToListAsync();

        // تخفیف‌های درصدی
        var discount1 = Discount.CreatePercentage(
            DiscountCode.Create("WELCOME20").Value,
            Title.Create("تخفیف خوش‌آمدگویی").Value,
            DiscountScope.Global,
            20,
            now,
            now.AddMonths(3)
        ).Value;
        discount1.SetDescription(Description.Create("تخفیف ۲۰٪ برای کاربران جدید").Value);
        discount1.SetMinimumOrderAmount(Money.Create(100000, "IRT").Value);
        discount1.SetUsageLimit(100);

        var discount2 = Discount.CreatePercentage(
            DiscountCode.Create("SUMMER50").Value,
            Title.Create("تخفیف تابستانه").Value,
            DiscountScope.Global,
            50,
            now,
            now.AddMonths(2)
        ).Value;
        discount2.SetMaximumDiscountAmount(Money.Create(500000, "IRT").Value);
        discount2.SetUsageLimit(50);

        var discount3 = Discount.CreatePercentage(
            DiscountCode.Create("VIP30").Value,
            Title.Create("تخفیف ویژه").Value,
            DiscountScope.SpecificCourses,
            30,
            now,
            now.AddMonths(1)
        ).Value;
        if (courses.Any())
            discount3.AddCourse(courses[0].Id);

        // تخفیف‌های مبلغ ثابت
        var discount4 = Discount.CreateFixedAmount(
            DiscountCode.Create("FIXED100K").Value,
            Title.Create("تخفیف ۱۰۰ هزار تومانی").Value,
            DiscountScope.Global,
            Money.Create(100000, "IRT").Value,
            now,
            now.AddMonths(2)
        ).Value;
        discount4.SetMinimumOrderAmount(Money.Create(500000, "IRT").Value);

        var discount5 = Discount.CreateFixedAmount(
            DiscountCode.Create("SAVE200K").Value,
            Title.Create("صرفه‌جویی ۲۰۰ هزار تومان").Value,
            DiscountScope.SpecificCategories,
            Money.Create(200000, "IRT").Value,
            now,
            now.AddMonths(1)
        ).Value;
        if (categories.Any())
            discount5.AddCategory(categories[0].Id);

        var discount6 = Discount.CreateFixedAmount(
            DiscountCode.Create("FIRST50K").Value,
            Title.Create("اولین خرید").Value,
            DiscountScope.Global,
            Money.Create(50000, "IRT").Value,
            now,
            now.AddYears(1)
        ).Value;
        discount6.MakeSingleUse();

        context.Discounts.AddRange(discount1, discount2, discount3, discount4, discount5, discount6);
        await context.SaveChangesAsync();
    }
#region SeedCoursesAsync

private async Task SeedCoursesAsync()
{
    if (await context.Courses.AnyAsync())
        return;

    var instructors = await context.Instructors.ToListAsync();
    var tags = await context.Tags.ToListAsync();

    // Load all subcategories (those with ParentId) for assigning to courses
    var subCategories = await context.Categories
        .Where(c => c.ParentCategoryId != null)
        .ToListAsync();

    if (!instructors.Any() || !subCategories.Any()) return;

    #region Helper Methods

    // Pick a random instructor
    Instructor RandomInstructor() => instructors[Random.Shared.Next(instructors.Count)];

    // Pick 1-3 random tags
    List<Tag> RandomTags() => tags
        .OrderBy(_ => Random.Shared.Next())
        .Take(Random.Shared.Next(1, 4))
        .ToList();

    #endregion

    #region Course Definitions (grouped by subcategory slug)

    // All desc >= 20 chars, all shortDesc >= 20 chars
    var courseDefinitions =
        new List<(string subCategorySlug, string title, string desc, string shortDesc, string courseSlug,
            CourseLevel level)>
        {
            // ── Programming > Web Development ──
            ("web-development", "آموزش جامع ASP.NET Core",
                "یادگیری کامل فریمورک ASP.NET Core از سطح مبتدی تا پیشرفته همراه با پروژه‌های عملی و کاربردی",
                "آموزش صفر تا صد فریمورک ASP.NET Core برای توسعه وب",
                "aspnet-core-course", CourseLevel.Beginner),

            ("web-development", "توسعه وب با React و Next.js",
                "ساخت اپلیکیشن‌های مدرن و پرسرعت وب با استفاده از کتابخانه React و فریمورک Next.js به صورت پروژه‌محور",
                "آموزش کامل React و Next.js برای ساخت وب‌اپلیکیشن مدرن",
                "react-nextjs-course", CourseLevel.Advanced),

            ("web-development", "آموزش Vue.js 3 از صفر",
                "یادگیری فریمورک Vue.js نسخه سوم با Composition API و ساخت پروژه واقعی فروشگاهی",
                "آموزش کامل Vue.js 3 با پروژه عملی و Composition API",
                "vuejs3-course", CourseLevel.Intermediate),

            ("web-development", "آموزش کامل Blazor و WebAssembly",
                "ساخت اپلیکیشن‌های تعاملی سمت کلاینت با Blazor WebAssembly و سی‌شارپ بدون نیاز به جاوااسکریپت",
                "آموزش Blazor و WebAssembly برای توسعه‌دهندگان دات‌نت",
                "blazor-wasm-course", CourseLevel.Intermediate),

            // ── Programming > Mobile Development ──
            ("mobile-development", "برنامه‌نویسی Flutter از صفر",
                "ساخت اپلیکیشن‌های کراس‌پلتفرم اندروید و iOS با فریمورک Flutter و زبان برنامه‌نویسی Dart",
                "آموزش جامع Flutter و Dart برای ساخت اپلیکیشن موبایل",
                "flutter-course", CourseLevel.Beginner),

            ("mobile-development", "توسعه اپلیکیشن با React Native",
                "ساخت اپلیکیشن موبایل اندروید و iOS با React Native و TypeScript به همراه پروژه عملی",
                "آموزش React Native و TypeScript برای توسعه اپ موبایل",
                "react-native-course", CourseLevel.Intermediate),

            ("mobile-development", "آموزش MAUI برای توسعه موبایل",
                "یادگیری فریمورک دات‌نت MAUI برای ساخت اپلیکیشن‌های کراس‌پلتفرم موبایل و دسکتاپ با سی‌شارپ",
                "آموزش جامع دات‌نت MAUI برای اپلیکیشن‌های چندسکویی",
                "maui-course", CourseLevel.Intermediate),

            // ── Programming > Database ──
            ("database", "آموزش پایگاه داده و SQL Server",
                "طراحی، پیاده‌سازی و مدیریت پایگاه‌های داده رابطه‌ای با Microsoft SQL Server به همراه کوئری‌نویسی پیشرفته",
                "آموزش کامل SQL Server و طراحی پایگاه داده رابطه‌ای",
                "sql-server-course", CourseLevel.Beginner),

            ("database", "آموزش MongoDB و NoSQL",
                "یادگیری طراحی و مدیریت دیتابیس‌های غیررابطه‌ای سندمحور با MongoDB به همراه تمرین‌های عملی",
                "آموزش MongoDB و اصول طراحی دیتابیس‌های NoSQL",
                "mongodb-course", CourseLevel.Intermediate),

            // ── Design > UI/UX ──
            ("ui-ux-design", "طراحی رابط کاربری با Figma",
                "یادگیری اصول و تکنیک‌های حرفه‌ای طراحی رابط کاربری و تجربه کاربری با ابزار محبوب Figma",
                "آموزش طراحی UI/UX با ابزار Figma از مقدماتی تا پیشرفته",
                "figma-uiux-course", CourseLevel.Beginner),

            ("ui-ux-design", "طراحی تجربه کاربری پیشرفته",
                "تحقیقات کاربری حرفه‌ای، ساخت پرسونا، نقشه سفر کاربر و تست قابلیت استفاده برای محصولات دیجیتال",
                "آموزش UX Research و تحقیقات کاربری پیشرفته",
                "ux-research-course", CourseLevel.Advanced),

            // ── Design > Graphic Design ──
            ("graphic-design", "آموزش فتوشاپ حرفه‌ای",
                "یادگیری کامل نرم‌افزار Adobe Photoshop برای طراحی گرافیک، رتوش تصاویر و ساخت بنرهای تبلیغاتی",
                "آموزش Adobe Photoshop از مقدماتی تا سطح حرفه‌ای",
                "photoshop-course", CourseLevel.Beginner),

            ("graphic-design", "طراحی لوگو با Illustrator",
                "اصول و تکنیک‌های طراحی لوگوی حرفه‌ای و هویت بصری برند با نرم‌افزار Adobe Illustrator",
                "آموزش طراحی لوگو و برندینگ با Adobe Illustrator",
                "logo-design-course", CourseLevel.Intermediate),

            // ── Design > Motion Graphics ──
            ("motion-graphics", "آموزش After Effects از صفر",
                "ساخت موشن گرافیک و انیمیشن‌های جذاب تبلیغاتی با نرم‌افزار Adobe After Effects به صورت پروژه‌محور",
                "آموزش موشن گرافیک با After Effects از صفر تا صد",
                "after-effects-course", CourseLevel.Beginner),

            ("motion-graphics", "انیمیشن سه‌بعدی با Cinema 4D",
                "یادگیری ساخت انیمیشن‌ها و صحنه‌های سه‌بعدی حرفه‌ای با نرم‌افزار Maxon Cinema 4D",
                "آموزش ساخت انیمیشن سه‌بعدی حرفه‌ای با Cinema 4D",
                "cinema4d-course", CourseLevel.Advanced),

            // ── Business > Project Management ──
            ("project-management", "مدیریت پروژه با Scrum",
                "یادگیری فریمورک اسکرام و اصول متدولوژی چابک (Agile) برای مدیریت حرفه‌ای پروژه‌های نرم‌افزاری",
                "آموزش Scrum و متدولوژی Agile برای مدیریت پروژه",
                "scrum-agile-course", CourseLevel.Beginner),

            ("project-management", "آمادگی آزمون PMP",
                "دوره جامع و کاربردی آمادگی برای آزمون بین‌المللی مدیریت پروژه PMP با تست‌های تمرینی فراوان",
                "آموزش آمادگی آزمون بین‌المللی مدیریت پروژه PMP",
                "pmp-course", CourseLevel.Advanced),

            // ── Business > Entrepreneurship ──
            ("entrepreneurship", "کارآفرینی و راه‌اندازی استارتاپ",
                "از ایده‌پردازی تا اجرا و جذب سرمایه: راهنمای جامع و عملی راه‌اندازی کسب‌وکار نوپا",
                "آموزش کارآفرینی و راه‌اندازی استارتاپ از صفر تا صد",
                "startup-course", CourseLevel.Beginner),

            ("entrepreneurship", "مدل کسب‌وکار و بوم ناب",
                "طراحی و اعتبارسنجی مدل کسب‌وکار با ابزارهای Business Model Canvas و Lean Canvas",
                "آموزش طراحی مدل کسب‌وکار با بوم ناب و بوم مدل",
                "business-model-course", CourseLevel.Intermediate),

            // ── Marketing > SEO ──
            ("seo", "آموزش سئو از صفر تا صد",
                "بهینه‌سازی وب‌سایت برای موتورهای جستجو، افزایش ترافیک ارگانیک و کسب رتبه‌های بالاتر در گوگل",
                "آموزش جامع سئو و بهینه‌سازی وب‌سایت برای گوگل",
                "seo-course", CourseLevel.Beginner),

            ("seo", "سئوی تکنیکال پیشرفته",
                "بهینه‌سازی Core Web Vitals، پیاده‌سازی Schema Markup و تکنیک‌های فنی پیشرفته سئوی وب‌سایت",
                "آموزش Technical SEO و بهینه‌سازی فنی پیشرفته سایت",
                "technical-seo-course", CourseLevel.Advanced),

            // ── Marketing > Social Media ──
            ("social-media", "بازاریابی در اینستاگرام",
                "استراتژی‌های حرفه‌ای رشد فالوور، تولید محتوا و افزایش فروش در پلتفرم اینستاگرام",
                "آموزش مارکتینگ و بازاریابی حرفه‌ای در اینستاگرام",
                "instagram-marketing-course", CourseLevel.Beginner),

            ("social-media", "تبلیغات در گوگل ادز",
                "راه‌اندازی، مدیریت و بهینه‌سازی کمپین‌های تبلیغاتی در پلتفرم Google Ads برای جذب مشتری",
                "آموزش راه‌اندازی و بهینه‌سازی تبلیغات در Google Ads",
                "google-ads-course", CourseLevel.Intermediate),

            // ── Lifestyle > Fitness ──
            ("fitness", "تناسب اندام و بدنسازی",
                "برنامه‌ریزی تمرینی و تغذیه‌ای اصولی برای رسیدن به تناسب اندام و افزایش قدرت بدنی",
                "آموزش بدنسازی و تناسب اندام با برنامه تمرینی اصولی",
                "fitness-course", CourseLevel.Beginner),

            ("fitness", "یوگا و مدیتیشن برای همه",
                "آموزش حرکات یوگا و تکنیک‌های مدیتیشن برای آرامش ذهنی و سلامت جسمانی در زندگی روزمره",
                "آموزش یوگا و مدیتیشن برای آرامش و سلامت بدن",
                "yoga-course", CourseLevel.Beginner),

            // ── Lifestyle > Cooking ──
            ("cooking", "آشپزی ایرانی سنتی",
                "آموزش گام‌به‌گام پخت غذاهای اصیل و سنتی ایرانی با نکات و فوت‌وفن‌های آشپزی حرفه‌ای",
                "آموزش آشپزی سنتی ایرانی با دستورپخت‌های اصیل و خوشمزه",
                "iranian-cooking-course", CourseLevel.Beginner),

            ("cooking", "شیرینی‌پزی حرفه‌ای",
                "آموزش پخت انواع کیک، شیرینی و دسرهای حرفه‌ای خانگی با تکنیک‌های قنادی مدرن",
                "آموزش شیرینی‌پزی و قنادی حرفه‌ای با تکنیک‌های مدرن",
                "pastry-course", CourseLevel.Intermediate),

            // ── Language > English ──
            ("english", "آموزش زبان انگلیسی مقدماتی",
                "یادگیری اصول گرامر، واژگان پرکاربرد و مکالمه روزمره انگلیسی از سطح پایه تا متوسط",
                "آموزش زبان انگلیسی از سطح مقدماتی با گرامر و مکالمه",
                "english-beginner-course", CourseLevel.Beginner),

            ("english", "آمادگی آزمون IELTS",
                "دوره جامع آمادگی برای چهار مهارت آزمون آیلتس شامل Listening، Reading، Writing و Speaking",
                "آموزش آمادگی کامل آزمون آیلتس با تمرین چهار مهارت",
                "ielts-course", CourseLevel.Advanced),

            // ── Language > German ──
            ("german", "آموزش زبان آلمانی A1-A2",
                "یادگیری زبان آلمانی از سطح مقدماتی A1 تا A2 با تمرکز بر گرامر، واژگان و مکالمات روزانه",
                "آموزش زبان آلمانی سطح مقدماتی از پایه تا سطح A2",
                "german-beginner-course", CourseLevel.Beginner),

            ("german", "آمادگی آزمون Goethe B1",
                "دوره تخصصی آمادگی برای آزمون گوته سطح B1 شامل تمرین مهارت‌های چهارگانه و نکات آزمون",
                "آموزش آمادگی آزمون گوته‌انستیتوت سطح B1 آلمانی",
                "goethe-b1-course", CourseLevel.Intermediate),
        };

    #endregion

    #region Section & Episode Templates

    var sectionTemplates = new (string Title, (string EpTitle, int Minutes, int Price)[] Episodes)[]
    {
        ("مقدمه و آشنایی", new[]
        {
            ("معرفی دوره و پیش‌نیازها", 10, 0),
            ("نصب و راه‌اندازی ابزارها", 15, 0)
        }),
        ("مفاهیم پایه", new[]
        {
            ("مفاهیم اولیه و اصطلاحات", 25, 75000),
            ("تمرین عملی اول", 30, 90000)
        }),
        ("مباحث پیشرفته", new[]
        {
            ("تکنیک‌های پیشرفته", 40, 150000),
            ("پروژه عملی نهایی", 50, 200000)
        }),
    };

    #endregion

    #region Build Lookup & Create Courses

    // Build a lookup: slug -> Category
    var categoryLookup =
        subCategories.ToDictionary<Category, string, Category>(c => c.Slug, c => c);

    var allCourses = new List<Course>();
    var allCourseCategories = new List<CourseCategory>();
    var allCourseTags = new List<CourseTag>();

    foreach (var def in courseDefinitions)
    {
        // Find the matching subcategory by slug
        if (!categoryLookup.TryGetValue(def.subCategorySlug, out var category))
            continue;

        var courseResult = Course.Create(
            def.title,
            def.desc,
            def.shortDesc,
            def.courseSlug,
            def.level,
            RandomInstructor().Id,
            "IRT"
        );

        if (!courseResult.IsSuccess) continue;

        var course = courseResult.Value;

        // Set default course images
        course.SetCourseImage(
            CourseImageName.Create("default.png").Value,
            CourseThumbnailName.Create("default.png").Value
        );

        // Create CourseCategory junction record (one category per course)
        allCourseCategories.Add(
            CourseCategory.Create(course.Id, category.Id)
        );

        // Create CourseTag junction records (1-3 random tags)
        foreach (var tag in RandomTags())
        {
            // Prevent duplicate tag assignments for same course
            if (allCourseTags.Any(ct => ct.CourseId == course.Id && ct.TagId == tag.Id))
                continue;

            allCourseTags.Add(
                CourseTag.Create(course.Id, tag.Id)
            );
        }

        // Add sections and episodes
        for (int s = 0; s < sectionTemplates.Length; s++)
        {
            var (sectionTitle, episodes) = sectionTemplates[s];
            var sectionResult = course.AddSection(sectionTitle, s + 1);
            if (!sectionResult.IsSuccess) continue;

            var section = sectionResult.Value;
            foreach (var (epTitle, minutes, price) in episodes)
            {
                course.AddEpisode(
                    section.Id,
                    EpisodeId.New(),
                    EpisodeTitle.Create(epTitle).Value,
                    TimeSpan.FromMinutes(minutes),
                    Money.Create(price).Value,
                    null
                );
            }
        }

        // Publish ~90% of courses, leave ~10% as draft
        if (Random.Shared.Next(10) > 0)
            course.Publish();

        allCourses.Add(course);
    }

    #endregion

    #region Save All To Database

    context.Courses.AddRange(allCourses);
    context.Set<CourseCategory>().AddRange(allCourseCategories);
    context.Set<CourseTag>().AddRange(allCourseTags);

    await context.SaveChangesAsync();

    #endregion
}

#endregion

    private async Task SeedCourseRatingsAsync()
    {
        if (await context.CourseRatings.AnyAsync())
            return;

        var users = (await context.Users.ToListAsync())
            .Where(u => !string.Equals(u.UserName.Value, "admin", StringComparison.Ordinal))
            .Take(8)
            .ToList();
        var courses = await context.Courses
            .Where(c => c.Status == CourseStatus.Published)
            .ToListAsync();

        if (!users.Any() || !courses.Any())
            return;

        var ratings = new List<CourseRating>();

        // هر دوره 2-5 رتبه‌بندی می‌گیره
        foreach (var course in courses)
        {
            var ratingCount = Random.Shared.Next(2, 6);
            var selectedUsers = users.OrderBy(_ => Random.Shared.Next()).Take(ratingCount);

            foreach (var user in selectedUsers)
            {
                var ratingValue = Random.Shared.Next(3, 6); // 3 تا 5 ستاره
                var comments = new[]
                {
                    "دوره عالی و کاربردی بود",
                    "مدرس خیلی خوب توضیح می‌داد",
                    "محتوای دوره به‌روز و مفید بود",
                    "پروژه‌های عملی خیلی کمک کرد",
                    "پیشنهاد می‌کنم حتما ببینید",
                    null
                };

                var comment = comments[Random.Shared.Next(comments.Length)];
                var ratingResult = CourseRating.Create(course.Id, user.Id, ratingValue, comment);
                if (ratingResult.IsSuccess)
                    ratings.Add(ratingResult.Value);
            }
        }

        context.CourseRatings.AddRange(ratings);
        await context.SaveChangesAsync();
    }

    private async Task SeedInstructorRatingsAsync()
    {
        if (await context.InstructorRatings.AnyAsync())
            return;

        var users = (await context.Users.ToListAsync())
            .Where(u => !string.Equals(u.UserName.Value, "admin", StringComparison.Ordinal))
            .Take(8)
            .ToList();

        var instructors = await context.Instructors.ToListAsync();

        if (!users.Any() || !instructors.Any())
            return;

        var ratings = new List<InstructorRating>();

        // هر مدرس 3-6 رتبه‌بندی می‌گیره
        foreach (var instructor in instructors)
        {
            var ratingCount = Random.Shared.Next(3, 7);
            var selectedUsers = users.OrderBy(_ => Random.Shared.Next()).Take(ratingCount);

            foreach (var user in selectedUsers)
            {
                var ratingValue = Random.Shared.Next(3, 6); // 3 تا 5 ستاره
                var comments = new[]
                {
                    "مدرس فوق‌العاده‌ای است",
                    "توضیحات بسیار واضح و روان",
                    "تجربه و تخصص بالایی دارد",
                    "پاسخگویی عالی به سوالات",
                    "روش تدریس حرفه‌ای",
                    null
                };

                var comment = comments[Random.Shared.Next(comments.Length)];
                var ratingResult = InstructorRating.Create(instructor.Id, user.Id, ratingValue, comment);

                if (ratingResult.IsSuccess)
                    ratings.Add(ratingResult.Value);
            }
        }

        context.InstructorRatings.AddRange(ratings);
        await context.SaveChangesAsync();
    }

    private async Task SeedOrdersAsync()
    {
        if (await context.Orders.AnyAsync()) return;

        var users = (await context.Users.ToListAsync())
            .Where(u => !string.Equals(u.UserName.Value, "admin", StringComparison.Ordinal)).Take(5)
            .ToList();
        var courses = await context.Courses
            .Include(c => c.Sections)
            .ThenInclude(s => s.Episodes)
            .Where(c => c.Status == CourseStatus.Published).Take(4).ToListAsync();
        var instructors = await context.Instructors.ToListAsync();

        if (!users.Any() || !courses.Any())
            return;
        var user1 = users[0];
        // سفارش 1: تکمیل شده
        var order1Result = Order.Create(user1.Id,user1.UserFullName,user1.Email,user1.PhoneNumber);
        if (order1Result.IsSuccess)
        {
            Order order1 = order1Result.Value;

            var course1 = courses[0];
            order1.AddItem(course1.Id,course1.Title,course1.ImageName, user1.UserFullName,course1.Price);

            if (courses.Count > 1)
            {

                var course2 = courses[1];
                order1.AddItem(course2.Id,course2.Title,course2.ImageName, user1.UserFullName,course2.Price);
            }

            var wallet1 = await context.Wallets.FirstOrDefaultAsync(w => w.UserId.Equals(users[0].Id));
            if (wallet1 != null)
            {
                var paymentResult =
                    wallet1.WithdrawFunds(order1.FinalAmount.Value, $"پرداخت سفارش {order1.OrderNumber.Value}");
                if (paymentResult.IsSuccess)
                {
                    var transaction1 = paymentResult.Value;
                    transaction1.AssignToOrder(order1.Id);
                    transaction1.MarkCompleted(0);
                    context.PaymentTransactions.Add(transaction1);

                    order1.ProcessPayment(transaction1.Id, PaymentMethod.Wallet);
                    order1.Complete(transaction1.Id,transaction1.OrderId,TransactionStatus.Completed);
                }
            }

            context.Orders.Add(order1);
        }

        // سفارش 2: در حال پردازش
        if (users.Count > 1 && courses.Count > 1)
        {
            var user2 = users[1];
            // Fix: pass user fields explicitly like order1
            var order2Result = Order.Create(user2.Id, user2.UserFullName, user2.Email, user2.PhoneNumber);
            if (order2Result.IsSuccess)
            {
                var order2 = order2Result.Value;

                var course2 = courses[1];
                // Fix: pass course fields explicitly like order1
                order2.AddItem(course2.Id, course2.Title, course2.ImageName,user2.UserFullName, course2.Price);

                var wallet2 = await context.Wallets.FirstOrDefaultAsync(w => w.UserId.Equals(user2.Id));
                if (wallet2 != null)
                {
                    var paymentResult = wallet2.WithdrawFunds(order2.FinalAmount.Value,
                        $"پرداخت سفارش {order2.OrderNumber.Value}");
                    if (paymentResult.IsSuccess)
                    {
                        var transaction2 = paymentResult.Value;
                        transaction2.AssignToOrder(order2.Id);
                        context.PaymentTransactions.Add(transaction2);

                        order2.ProcessPayment(transaction2.Id, PaymentMethod.Wallet);
                    }
                }

                context.Orders.Add(order2);
            }
        }

        // سفارش 3: در انتظار پرداخت
        if (users.Count > 2 && courses.Count > 2)
        {
            var user3 = users[2];
            // Fix: pass user fields explicitly like order1
            var order3Result = Order.Create(user3.Id, user3.UserFullName, user3.Email, user3.PhoneNumber);
            if (order3Result.IsSuccess)
            {
                var order3 = order3Result.Value;

                // Fix: pass course fields explicitly like order1
                order3.AddItem(courses[2].Id, courses[2].Title, courses[2].ImageName, user3.UserFullName, courses[2].Price);
                order3.SetCustomerNote("لطفا بعد از تایید پرداخت، دسترسی را فعال کنید");

                context.Orders.Add(order3);
            }
        }
// سفارش 4: لغو شده
        if (users.Count > 3 && courses.Count > 3)
        {
            var user4 = users[3];
            // Fix: pass user fields explicitly like order1
            var order4Result = Order.Create(user4.Id, user4.UserFullName, user4.Email, user4.PhoneNumber);
            if (order4Result.IsSuccess)
            {
                var order4 = order4Result.Value;

                // Fix: pass course fields explicitly like order1
                order4.AddItem(courses[3].Id, courses[3].Title, courses[3].ImageName, user4.UserFullName, courses[3].Price);
                order4.Cancel("کاربر درخواست لغو داد");

                context.Orders.Add(order4);
            }
        }


        await context.SaveChangesAsync();
    }
}
