<div dir="rtl">

# Vitastic.Domain 🧱

هسته اصلی پروژه — تمام منطق کسب‌وکار، قراردادها و مدل‌های پایه اینجا تعریف می‌شوند.
Domain به هیچ لایه‌ای وابسته نیست؛ لایه‌های Application و Infrastructure به Domain وابسته‌اند.

---

## 📋 فهرست مطالب

- [ساختار پوشه‌ها](#️-ساختار-پوشهها)
- [سلسله‌مراتب Entity](#-سلسلهمراتب-entity)
- [Strongly Typed Id](#-strongly-typed-id)
- [Value Objects](#-value-objects)
- [Result Pattern](#-result-pattern)
- [Domain Events](#-domain-events)
- [Exceptions](#-exceptions)
- [Repositories](#-repositories)
- [Helpers](#-helpers)

---

## 🗂️ ساختار پوشه‌ها

```
Vitastic.Domain/
├── Entities/
│   ├── Cart/
│   ├── Categories/
│   ├── Courses/
│   ├── Discounts/
│   ├── Instructors/
│   ├── Orders/
│   ├── Roles/
│   ├── Tags/
│   ├── Transactions/
│   ├── Users/
│   └── Wallets/
└── Shared/
    ├── Events/
    ├── Exceptions/
    ├── Helpers/
    ├── Models/          ← کلاس‌های پایه Entity و Id
    ├── Repositories/    ← قراردادهای Repository
    ├── Results/         ← Result Pattern
    └── ValueObjects/    ← Value Objectهای مشترک
```

---

## 🏛️ سلسله‌مراتب Entity

### نمودار ارث‌بری

```
IAuditableEntity
      │
      ▼
BaseEntity<TKey>          ← Id، Audit، Equality
      │
      ▼
FullEntity<TKey>          ← + SoftDelete (MarkDeleted / Restore)
      │
      ▼
AggregateRoot<TKey>       ← + Domain Events (RaiseDomainEvent)
```

همچنین `ISoftDeleteEntity` توسط `FullEntity` پیاده‌سازی می‌شود،
و `IAggregateRoot` توسط `AggregateRoot`.

---

### راهنمای انتخاب کلاس پایه

| نیاز Entity | کلاس پایه |
|---|---|
| فقط Id و اطلاعات Audit (CreatedBy، UpdatedBy) | `BaseEntity<TKey>` |
| Audit + قابلیت حذف نرم (SoftDelete) | `FullEntity<TKey>` |
| SoftDelete + انتشار Domain Event | `AggregateRoot<TKey>` |

> ⚠️ **قانون:** Entity‌هایی که رویداد دامنه منتشر می‌کنند **باید** از `AggregateRoot` ارث ببرند.
> Entity‌هایی که فقط داده نگه می‌دارند و رویداد ندارند، از `BaseEntity` یا `FullEntity` کافی است.

---

### مثال — تعریف Entity جدید

```csharp
// SoftDelete لازم دارد، رویداد ندارد
public class Category : FullEntity<CategoryId>
{
    private Category() { } // For EF Core
    private Category(CategoryId id) : base(id) { }

    public static Category Create(CategoryId id) => new(id);
}

// رویداد Domain دارد → AggregateRoot
public class User : AggregateRoot<UserId>
{
    private User() { } // For EF Core
    private User(UserId id) : base(id) { }

    public void Register()
    {
        RaiseDomainEvent(UserRegisteredDomainEvent.Create(...));
    }
}
```

---

## 🔑 Strongly Typed Id

به جای استفاده مستقیم از `Guid` یا `int` به عنوان Id، هر Entity یک نوع Id اختصاصی دارد.
این کار از اشتباه پاس دادن `UserId` به جای `OrderId` در زمان کامپایل جلوگیری می‌کند.

### سلسله‌مراتب Id

```
ValueObject<TValue>
      │
      ▼
StronglyTypedId<TId, TValue>
      │
      ├──► GuidBasedId<TId>     ← برای Entity‌هایی با Guid Id
      │
      └──► IntBasedId<TId>      ← برای Entity‌هایی با int Id
```

### راهنمای انتخاب

| نوع Id | کلاس پایه |
|---|---|
| `Guid` | `GuidBasedId<TId>` |
| `int` | `IntBasedId<TId>` |

### مثال — تعریف Id جدید

```csharp
// Id مبتنی بر Guid
public sealed class TagId : GuidBasedId<TagId>
{
    private TagId(Guid value) : base(value) { }

    public static TagId New() => new(Guid.NewGuid());

    public static Result<TagId> CreateFrom(Guid value) =>
        CreateFrom(value, v => new TagId(v), BaseIdErrors.Empty);

    public static Result<TagId> CreateFrom(string value) =>
        CreateFrom(value, v => new TagId(v), BaseIdErrors.Empty, BaseIdErrors.InvalidFormat(value));
}
```

---

## 💎 Value Objects

مقادیری که هویت ندارند و فقط با مقدارشان مقایسه می‌شوند.
همه از `ValueObject<TValueType>` یا `ValueObject` ارث می‌برند.

### Value Objectهای مشترک (Shared)

| نام | توضیح |
|---|---|
| `Address` | آدرس کامل |
| `Currency` | واحد پول (استاندارد ISO 4217) |
| `Description` | توضیحات |
| `Email` | آدرس ایمیل |
| `FullName` | نام و نام‌خانوادگی |
| `Money` | مبلغ + واحد پول |
| `PhoneNumber` | شماره تلفن |
| `Slug` | آدرس URL-friendly |
| `Title` | عنوان |

### مثال — تعریف Value Object جدید

```csharp
public sealed class Title : ValueObject<string>
{
    public const int MaxLength = 150;

    private Title(string value) : base(value) { }

    public static Result<Title> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Title.Empty", "عنوان نمی‌تواند خالی باشد");

        if (value.Length > MaxLength)
            return Error.Validation("Title.TooLong", $"عنوان نمی‌تواند بیشتر از {MaxLength} کاراکتر باشد");

        return new Title(value.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
```

> **قانون:** Value Object هرگز Id ندارد. سازنده `private` است و فقط از طریق Factory Method (`Create`) ساخته می‌شود.

---

## ✅ Result Pattern

به جای پرتاب Exception برای خطاهای قابل پیش‌بینی، از `Result` استفاده می‌شود.

### انواع خطا

| نوع | متد | کاربرد |
|---|---|---|
| `Validation` | `Error.Validation(code, message)` | ورودی نامعتبر |
| `NotFound` | `Error.NotFound(code, message)` | رکورد پیدا نشد |
| `Conflict` | `Error.Conflict(code, message)` | تداخل وضعیت |
| `Unauthorized` | `Error.Unauthorized()` | احراز هویت نشده |
| `Forbidden` | `Error.Forbidden()` | دسترسی ندارد |
| `Failure` | `Error.Failure(code, message)` | خطای عمومی |
| `Verification` | `Error.Verification(code, message)` | خطای اعتبارسنجی |

### مثال — استفاده در Entity

```csharp
// برگرداندن خطا
public Result MarkDeleted(Guid deletedBy)
{
    if (IsDeleted)
        return Error.Conflict("Entity.AlreadyDeleted", "این موجودیت قبلاً حذف شده است");

    IsDeleted = true;
    return Result.Success();
}

// برگرداندن مقدار + خطا
public static Result<TagId> CreateFrom(Guid value)
{
    if (value == Guid.Empty)
        return BaseIdErrors.Empty;      // implicit conversion از Error به Result<T>

    return new TagId(value);            // implicit conversion از TValue به Result<T>
}
```

---

## 📣 Domain Events

رویدادهایی که درون Domain اتفاق می‌افتند و لایه Application باید به آن‌ها واکنش نشان دهد.
پیاده‌سازی از MediatR's `INotification` استفاده می‌کند.

### ساختار پایه

```csharp
public abstract record DomainEvent(Guid Id, DateTimeOffset OccurredOn) : IDomainEvent;
```

### مثال — تعریف Event جدید

```csharp
public sealed record UserRegisteredDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string Email { get; init; }

    // سازنده برای Deserialization
    [JsonConstructor]
    [Obsolete("Use Create() factory method.")]
    public UserRegisteredDomainEvent(Guid userId, string email) 
    { 
        UserId = userId; 
        Email = email; 
    }

    // Factory Method — Value Objectها را به primitive تبدیل می‌کند
    public static UserRegisteredDomainEvent Create(UserId userId, Email email)
        => new(userId.Value, email.Value);
}
```

### مثال — انتشار Event در AggregateRoot

```csharp
public class User : AggregateRoot<UserId>
{
    public void Register(Email email)
    {
        // منطق ثبت‌نام ...
        RaiseDomainEvent(UserRegisteredDomainEvent.Create(Id, email));
    }
}
```

> **قانون:** Event در لایه Domain فقط `raise` می‌شود — هیچ‌گاه Handle نمی‌شود.
> Handle کردن Event وظیفه لایه Application است.

---

## ⚠️ Exceptions

برای خطاهای غیرقابل پیش‌بینی که نباید با Result مدیریت شوند.

> **قانون:** برای خطاهای قابل پیش‌بینی (مثل ورودی نامعتبر) از `Result` استفاده کن.
> Exception فقط برای موارد واقعاً استثنایی است (نقض قوانین بنیادی دامنه).

### Exception‌های موجود

| Exception | کاربرد |
|---|---|
| `DomainException` | کلاس پایه — مستقیم استفاده نمی‌شود |
| `BusinessRuleViolationException` | نقض قانون کسب‌وکار |
| `DuplicateEntityException` | ثبت موجودیت تکراری |
| `EntityNotFoundException` | موجودیت پیدا نشد (در موارد بحرانی) |
| `ForbiddenException` | عدم دسترسی در سطح Domain |
| `UniqueConstraintViolatedException` | نقض یکتایی |
| `ValidationException` | خطای اعتبارسنجی بحرانی |

---

## 📦 Repositories

قراردادهای دسترسی به داده — پیاده‌سازی در `Vitastic.Infra` است.

### قرارداد پایه

```csharp
public interface IRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellation = default);
    Task<bool> IsExistAsync(TKey id, CancellationToken cancellation = default);
    Task AddAsync(TEntity entity, CancellationToken cancellation = default);
    Task UpdateAsync(TEntity entity, CancellationToken token = default);
    Task DeleteAsync(TKey id, CancellationToken cancellation = default);
    Task DeleteAsync(TEntity entity, CancellationToken cancellation = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellation = default);
}
```

### مثال — تعریف Repository اختصاصی

```csharp
// قرارداد در Domain
public interface ITagRepository : IRepository<Tag, TagId>
{
    Task<Tag?> GetTagByName(string name);
    Task<bool> IsExistByNameAsync(TagName tagName, CancellationToken cancellationToken);
}

// پیاده‌سازی در Vitastic.Infra
public class TagRepository : ITagRepository { ... }
```

---

## 🔧 Helpers

کلاس‌های کمکی مشترک در سطح Domain.

| Helper | کاربرد |
|---|---|
| `OrderIdJsonConvertor` | تبدیل `OrderId` در Serialization/Deserialization |
| `PasswordHasher` | هش کردن رمز عبور |

</div>