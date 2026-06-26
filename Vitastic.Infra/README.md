<div dir="rtl">

# Vitastic.Infra 🔧

لایه زیرساخت — پیاده‌سازی تمام قراردادهای تعریف‌شده در `Vitastic.Domain` و `Vitastic.App`.
این لایه به Domain و Application وابسته است؛ هیچ‌کدام از آن‌ها به Infra وابسته نیستند.

---

## 📋 فهرست مطالب

- [ساختار پوشه‌ها](#️-ساختار-پوشهها)
- [Data و Configurations](#-data-و-configurations)
- [Unit of Work](#-unit-of-work)
- [Repositories](#-repositories)
- [Services](#-services)
- [Outbox Pattern](#-outbox-pattern)
- [Background Jobs](#-background-jobs)
- [Exceptions](#-exceptions)
- [Specifications](#-specifications)
- [Settings](#️-settings)

---

## 🗂️ ساختار پوشه‌ها

```
Vitastic.Infra/
├── BackgroundJobs/
│   └── ProcessOutboxMessagesJob.cs
├── Data/
│   ├── Configurations/
│   │   ├── Base/                    ← کانفیگ پایه Entity‌ها
│   │   ├── Seed/                    ← داده‌های اولیه
│   │   └── Write/                   ← کانفیگ اختصاصی هر Entity
│   └── ApplicationWriteDbContext.cs
├── Exceptions/
├── Interceptors/
│   └── ConvertDomainEventToOutboxMessageInterceptor.cs
├── Migrations/
├── Outbox/
│   └── OutboxMessage.cs
├── Repositories/
├── Services/
│   ├── Base/                        ← سرویس‌های پایه (Email، JWT، Storage، ...)
│   └── Queries/                     ← پیاده‌سازی Read Queryها
├── Specifications/
├── Settings/
│   └── BackblazeSettings.cs
├── DependencyInjection.cs
├── DesignTimeContextFactory.cs
└── UnitOfWork.cs
```

---

## 🗄️ Data و Configurations

### کانفیگ پایه Entity‌ها

مشابه سلسله‌مراتب Entity در Domain، کانفیگ‌های EF Core نیز از یکدیگر ارث می‌برند:

```
BaseEntityConfiguration        ← PrimaryKey، Audit Columns، Index روی CreatedAt
        │
        ▼
FullEntityConfiguration        ← + SoftDelete Columns، QueryFilter، Index روی IsDeleted
        │
        ▼
AggregateRootConfiguration     ← + نادیده گرفتن DomainEvents (Ignore)
```

### راهنمای انتخاب کانفیگ پایه

| نوع Entity در Domain | کانفیگ پایه در Infra |
|---|---|
| `BaseEntity` | `BaseEntityConfiguration<TEntity, TKey>` |
| `FullEntity` | `FullEntityConfiguration<TEntity, TKey>` |
| `AggregateRoot` | `AggregateRootConfiguration<TEntity, TKey>` |

> ⚠️ **قانون:** کانفیگ هر Entity باید با کلاس پایه‌ای که Entity از آن ارث برده هماهنگ باشد.
> اگر Entity از `AggregateRoot` ارث برده، کانفیگش باید از `AggregateRootConfiguration` ارث ببرد.

### مثال — کانفیگ Entity جدید

```csharp
// Tag از AggregateRoot ارث برده → کانفیگ از AggregateRootConfiguration
public sealed class TagConfiguration : AggregateRootConfiguration<Tag, TagId>
{
    public override void Configure(EntityTypeBuilder<Tag> builder)
    {
        base.Configure(builder); // ← همیشه اول base فراخوانی شود

        builder.ToTable("Tags");

        // تبدیل Strongly Typed Id به نوع پایه
        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => TagId.CreateFrom(value).Value
            );

        // تبدیل Value Object به نوع پایه
        builder.Property(t => t.Name)
            .HasMaxLength(TagName.MaxLength)
            .HasConversion(
                name => name.Value,
                value => TagName.Create(value).Value
            );
    }
}
```

> **نکته:** تمام Strongly Typed Id‌ها و Value Objectها باید با `HasConversion` به نوع پایه‌شان تبدیل شوند تا EF Core بتواند آن‌ها را در دیتابیس ذخیره کند.

### نکته مهم — Ignore در DbContext

در `ApplicationWriteDbContext.OnModelCreating` تمام Value Objectها، Strongly Typed Id‌ها و کلاس‌های پایه‌شان به‌صورت صریح `Ignore` شده‌اند:

```csharp
modelBuilder.Ignore(typeof(ValueObject<>));
modelBuilder.Ignore(typeof(StronglyTypedId<,>));
modelBuilder.Ignore(typeof(GuidBasedId<>));
modelBuilder.Ignore<TagId>();
// ...
```

**چرا؟** EF Core به‌صورت پیش‌فرض هر کلاسی که در DbSet یا Navigation Property ظاهر شود را به عنوان Entity می‌شناسد و سعی می‌کند برایش جدول بسازد. بدون این Ignore‌ها، EF Core برای `TagId`، `Money`، `Email` و تمام Value Objectها جدول جداگانه می‌ساخت.

> **قانون:** هر Value Object یا Strongly Typed Id جدیدی که اضافه می‌کنی، باید در `OnModelCreating` نیز `Ignore` شود — وگرنه Migration خطا می‌دهد.

### QueryFilter خودکار برای SoftDelete

`FullEntityConfiguration` به‌صورت خودکار این فیلتر را اعمال می‌کند:

```csharp
builder.HasQueryFilter(e => !e.IsDeleted);
```

یعنی تمام Entity‌های حذف‌شده به‌صورت پیش‌فرض از نتایج Query حذف می‌شوند — بدون نیاز به فیلتر دستی در هر Query.

---

## 🔄 Unit of Work

مدیریت Transaction و ذخیره‌سازی تغییرات به‌صورت اتمیک.

```
BeginTransactionAsync()   ← شروع Transaction
      │
SaveChangesAsync()        ← ذخیره تغییرات
      │
CommitAsync()             ← تأیید نهایی (در صورت خطا → RollbackAsync خودکار)
```

### مثال — استفاده در Application Layer

```csharp
await _unitOfWork.BeginTransactionAsync();
// عملیات روی Entity‌ها ...
await _unitOfWork.CommitAsync();
```

> **نکته:** `CommitAsync` در صورت بروز خطا به‌صورت خودکار Rollback می‌کند و Exception را دوباره پرتاب می‌کند.

---

## 📦 Repositories

### کلاس پایه

تمام Repository‌ها از `BaseRepository<TEntity, TKey>` ارث می‌برند.
این کلاس عملیات CRUD پایه را پیاده‌سازی می‌کند و تمام خطاهای دیتابیس را به Exception‌های Domain/Infra تبدیل می‌کند.

```
IRepository<TEntity, TKey>         ← قرارداد در Domain
        │
        ▼
BaseRepository<TEntity, TKey>      ← پیاده‌سازی پایه در Infra
        │
        ▼
TagRepository, CategoryRepository, ...  ← Repository اختصاصی هر Entity
```

### تبدیل خودکار خطاهای دیتابیس

تمام عملیات از طریق متد `ExecuteAsync` رد می‌شوند که خطاهای EF Core را به Exception‌های معنادار تبدیل می‌کند:

| خطای EF Core | Exception تولیدشده |
|---|---|
| `UniqueConstraintException` | `UniqueConstraintViolatedException` |
| `DbUpdateConcurrencyException` | `ConcurrencyConflictException` |
| `CannotInsertNullException` | `RepositoryException` |
| `MaxLengthExceededException` | `RepositoryException` |
| `DbUpdateException` | `RepositoryException` |

### مثال — تعریف Repository اختصاصی

```csharp
internal class CategoryRepository(ApplicationWriteDbContext dbContext, ILogger<CategoryRepository> logger)
    : BaseRepository<Category, CategoryId>(dbContext), ICategoryRepository
{
    public async Task<Category?> GetCategoryByName(string name, CancellationToken ct)
    {
        // همیشه از ExecuteAsync استفاده کن تا خطاها مدیریت شوند
        return await ExecuteAsync(
            () => Context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.Value == name, ct),
            ct);
    }
}
```

---

## 🔌 Services

### سرویس‌های پایه (Base)

| سرویس | مسئولیت |
|---|---|
| `EmailSender` | ارسال ایمیل |
| `EmailBodyHelper` | ساخت قالب HTML ایمیل |
| `JwtTokenService` | تولید و اعتبارسنجی JWT |
| `InternalStorageService` | ذخیره فایل در Storage |
| `PaymentGatewayService` | ارتباط با درگاه پرداخت زرین‌پال |

### سرویس‌های Read Query

پیاده‌سازی interface‌های Read Query تعریف‌شده در `Vitastic.App` برای خواندن داده:

| سرویس | مسئولیت |
|---|---|
| `CategoryQueryService` | Query‌های خواندن دسته‌بندی |
| `CourseQueryService` | Query‌های خواندن دوره |
| `DiscountQueryService` | Query‌های خواندن تخفیف |
| `PaymentTransactionQuery` | Query‌های تراکنش پرداخت |
| ... | ... |

> **نکته:** سرویس‌های Query از `ApplicationWriteDbContext` با `AsNoTracking()` استفاده می‌کنند — چون فقط می‌خوانند و تغییری نمی‌دهند.

---

## 📬 Outbox Pattern

برای اطمینان از اینکه Domain Event‌ها حتی در صورت خرابی سیستم از دست نروند.

```
Entity تغییر می‌کند
        │
        ▼
ConvertDomainEventToOutboxMessageInterceptor   ← Interceptor EF Core
        │  (قبل از SaveChanges اجرا می‌شود)
        ▼
OutboxMessage در دیتابیس ذخیره می‌شود
        │
        ▼
ProcessOutboxMessagesJob                       ← Background Job
        │  (پیام‌های پردازش‌نشده را می‌خواند)
        ▼
Domain Event پردازش می‌شود
```

> **چرا Outbox؟** اگر Event مستقیم Publish شود و سرور قبل از پردازش خراب شود، Event از دست می‌رود.
> با Outbox، Event ابتدا در همان Transaction دیتابیس ذخیره می‌شود — پس هرگز از دست نمی‌رود.

---

## ⚙️ Background Jobs

| Job | وظیفه |
|---|---|
| `ProcessOutboxMessagesJob` | خواندن پیام‌های Outbox پردازش‌نشده و Publish کردن آن‌ها |

---

## ⚠️ Exceptions

| Exception | کاربرد |
|---|---|
| `InfrastructureException` | کلاس پایه — همه Exception‌های Infra از این ارث می‌برند |
| `RepositoryException` | خطاهای عمومی دیتابیس |
| `ConcurrencyConflictException` | دو کاربر یک رکورد را همزمان ویرایش کردند |
| `UniqueConstraintViolatedException` | نقض یکتایی در دیتابیس |
| `FileStorageException` | خطا در ذخیره یا دریافت فایل |
| `InternalStorageError` | خطای داخلی Storage |

---

## 🔍 Specifications

الگوی Specification برای캡ساله‌سازی منطق فیلترینگ Query.

| کلاس | مسئولیت |
|---|---|
| `Specification<TEntity, TKey>` | کلاس پایه |
| `SpecificationEvaluator` | اعمال Specification روی `IQueryable` |
| `UserWithRolesSpecification` | نمونه — کاربر به همراه نقش‌هایش |

> **وضعیت فعلی:** Specification فعلاً کاربرد محدودی دارد و در صورت نیاز به Query‌های پیچیده‌تر توسعه می‌یابد.

---

## ⚙️ Settings

| کلاس | مسئولیت |
|---|---|
| `BackblazeSettings` | تنظیمات اتصال به Backblaze B2 برای ذخیره فایل در فضای ابری |

> **وضعیت فعلی:** Storage فعلاً به‌صورت داخلی (Local) است. `BackblazeSettings` برای مهاجرت آینده به فضای ابری آماده شده است.

</div>