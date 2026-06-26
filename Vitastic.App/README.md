<div dir="rtl">

# Vitastic.App 🎯

لایه Application — تمام منطق Use Caseها، Commandها، Queryها و Handlerها اینجا تعریف می‌شوند.
این لایه به `Vitastic.Domain` وابسته است و هیچ وابستگی مستقیمی به Infra یا Web ندارد.

---

## 📋 فهرست مطالب

- [ساختار پوشه‌ها](#️-ساختار-پوشهها)
- [CQRS با MediatR](#-cqrs-با-mediatr)
- [Pipeline Behaviors](#-pipeline-behaviors)
- [Features — ساختار هر Use Case](#-features--ساختار-هر-use-case)
- [Mapping — AutoMapper Profiles](#️-mapping--automapper-profiles)
- [Abstractions — قراردادهای سرویس‌ها](#-abstractions--قراردادهای-سرویسها)
- [Data](#-data)
- [Settings](#️-settings)

---

## 🗂️ ساختار پوشه‌ها

```
Vitastic.App/
├── Common/
│   ├── Abstractions/
│   │   ├── Files/              ← جایگزین IFormFile
│   │   ├── Messaging/          ← ICommand، IQuery، IEventHandler
│   │   └── Services/
│   │       ├── Base/           ← قراردادهای سرویس‌های پایه
│   │       └── Queries/        ← قراردادهای Read Query سرویس‌ها
│   └── Behaviors/              ← Pipeline Behaviors
├── Data/
│   └── IUnitOfWork.cs
├── Features/
│   ├── Carts/
│   ├── Categories/
│   ├── Courses/
│   ├── Discounts/
│   ├── Instructors/
│   ├── Orders/
│   ├── Payments/
│   ├── Permissions/
│   ├── Roles/
│   ├── Tags/
│   ├── Users/
│   └── Wallets/
│       ├── Commands/
│       ├── Queries/
│       ├── Events/
│       └── Dtos/
├── Settings/
├── ApplicationAssemblyReference.cs
└── DependencyInjection.cs
```

---

## ⚡ CQRS با MediatR

تمام عملیات در این لایه از الگوی **CQRS** پیروی می‌کنند و با **Result Pattern** ترکیب شده‌اند.

### سلسله‌مراتب Command

```
IRequest<Result>              ← MediatR
        │
        ▼
ICommand                      ← Command بدون مقدار بازگشتی
ICommand<TResponse>           ← Command با مقدار بازگشتی

IRequestHandler               ← MediatR
        │
        ▼
ICommandHandler<TCommand>              ← Handler برای ICommand
ICommandHandler<TCommand, TResponse>   ← Handler برای ICommand<TResponse>
```

### سلسله‌مراتب Query

```
IRequest<Result<TResponse>>   ← MediatR
        │
        ▼
IQuery<TResponse>             ← Query با مقدار بازگشتی

IRequestHandler               ← MediatR
        │
        ▼
IQueryHandler<TQuery, TResponse>   ← Handler برای IQuery
```

### چرا Result Pattern روی MediatR؟

به جای پرتاب Exception برای خطاهای قابل پیش‌بینی، هر Command و Query یک `Result` یا `Result<T>` برمی‌گرداند. این کار:
- خطاها را بخشی صریح از قرارداد Handler می‌کند
- لایه API را مجبور می‌کند همیشه خطا را بررسی کند
- Exception فقط برای موارد واقعاً غیرمنتظره استفاده می‌شود

---

### مثال — تعریف Command جدید

```csharp
// Command
public sealed record CreateTagCommand(string Name) : ICommand<TagId>;

// Handler
internal sealed class CreateTagCommandHandler(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<CreateTagCommand, TagId>
{
    public async Task<Result<TagId>> Handle(
        CreateTagCommand request,
        CancellationToken cancellationToken)
    {
        // اعتبارسنجی
        var nameResult = TagName.Create(request.Name);
        if (nameResult.IsFailure)
            return nameResult.Error;

        // بررسی تکراری نبودن
        var exists = await tagRepository.IsExistByNameAsync(nameResult.Value, cancellationToken);
        if (exists)
            return Error.Conflict("Tag.Duplicate", "تگ با این نام قبلاً ثبت شده است");

        // ساخت و ذخیره
        var tag = Tag.Create(TagId.New(), nameResult.Value);
        await tagRepository.AddAsync(tag, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return tag.Id;
    }
}
```

### مثال — تعریف Query جدید

```csharp
// Query
public sealed record GetTagByIdQuery(Guid Id) : IQuery<TagDto>;

// Handler
internal sealed class GetTagByIdQueryHandler(ITagQueryService tagQuery)
    : IQueryHandler<GetTagByIdQuery, TagDto>
{
    public async Task<Result<TagDto>> Handle(
        GetTagByIdQuery request,
        CancellationToken cancellationToken)
    {
        var tagId = TagId.CreateFrom(request.Id);
        if (tagId.IsFailure)
            return tagId.Error;

        var tag = await tagQuery.GetByIdAsync(tagId.Value, cancellationToken);
        if (tag is null)
            return Error.NotFound("Tag.NotFound", "تگ مورد نظر یافت نشد");

        return tag;
    }
}
```

---

## 🔄 Pipeline Behaviors

قبل و بعد از اجرای هر Handler، به‌صورت خودکار اجرا می‌شوند:

```
Request ──► ValidationBehavior ──► LoggingBehavior ──► UnitOfWorkBehavior ──► Handler
                    │                                          │
             اعتبارسنجی                              SaveChanges خودکار
             FluentValidation                         (فقط برای Command)
```

| Behavior | مسئولیت |
|---|---|
| `ValidationPipelineBehavior` | اجرای خودکار Validatorهای FluentValidation قبل از Handler |
| `LoggingPipelineBehavior` | لاگ کردن ورودی، خروجی و زمان اجرای هر Request |
| `UnitOfWorkBehavior` | فراخوانی خودکار `SaveChanges` بعد از موفقیت Command |

> **نکته:** `UnitOfWorkBehavior` فقط روی `ICommand` اعمال می‌شود، نه `IQuery` — چون Query هیچ تغییری در داده ایجاد نمی‌کند.

---

## 📁 Features — ساختار هر Use Case

هر Feature (مثلاً Tags، Users، Courses) پوشه مستقل خودش را دارد:

```
Features/Tags/
├── Commands/
│   ├── CreateTag/
│   │   ├── CreateTagCommand.cs
│   │   ├── CreateTagCommandHandler.cs
│   │   └── CreateTagCommandValidator.cs
│   └── DeleteTag/
│       ├── DeleteTagCommand.cs
│       └── DeleteTagCommandHandler.cs
├── Queries/
│   ├── GetTagById/
│   │   ├── GetTagByIdQuery.cs
│   │   └── GetTagByIdQueryHandler.cs
│   └── GetAllTags/
├── Events/
│   └── TagCreatedDomainEventHandler.cs   ← واکنش به Domain Event
└── Dtos/
    └── TagDto.cs                         ← داده خروجی Handler به لایه API
```

### Dto

هر Feature Dto مخصوص خودش را دارد — داده‌ای که Handler به لایه API برمی‌گرداند:

```csharp
// ساختار معمول Dto — فقط primitive typeها، بدون Value Object
public record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int UsageCount { get; init; }
    public bool IsActive { get; init; }
}
```

> **قانون:** Dto هرگز Value Object یا Entity ندارد — فقط نوع‌های ساده (`Guid`، `string`، `int`، ...).
> تبدیل از Entity به Dto از طریق AutoMapper و Mapping Profileها انجام می‌شود.

### Event Handler

Event‌ها از طریق **Outbox Pattern** پردازش می‌شوند. Handler می‌تواند خودش Command جدیدی به MediatR بفرستد:

```csharp
public class CreateUserWalletOnUserCreatedByAdminDomainEvent(
    ILogger<...> logger,
    IMediator mediator)
    : IEventHandler<UserCreatedByAdminDomainEvent>
{
    public async Task Handle(UserCreatedByAdminDomainEvent notification, CancellationToken token)
    {
        // ارسال Command جدید از داخل Event Handler
        Result<Guid> result = await mediator.Send(
            new CreateWalletCommand(notification.UserId), token);

        if (result.IsFailure)
            logger.LogCritical("ساخت کیف پول شکست خورد: {Error}", result.Error.Code);
    }
}
```

> **نکته:** Event‌ها از طریق Outbox Pattern (در لایه Infra) پردازش می‌شوند، نه مستقیم.
> این تضمین می‌کند که حتی در صورت خرابی سرور، Event از دست نرود.

---

## 🗺️ Mapping — AutoMapper Profiles

### ساختار Mapping

```
Common/Mapping/
├── BaseMappingProfile.cs        ← تبدیل Value Object → primitive type (برای همه لایه‌ها)
├── CategoryMappingProfile.cs    ← Entity → Dto اختصاصی هر Feature
├── CourseMappingProfile.cs
├── InstructorMappingProfile.cs
├── TagMappingProfile.cs
├── UserMappingProfile.cs
└── ...
```

### BaseMappingProfile — تبدیل Value Object

تمام Value Objectها یک‌بار اینجا به نوع پایه‌شان Map می‌شوند و در تمام Profile‌های دیگر قابل استفاده هستند:

```csharp
// نمونه‌هایی از BaseMappingProfile
CreateMap<Email, string>().ConvertUsing(src => src.Value);
CreateMap<Money, decimal>().ConvertUsing(src => src.Value);
CreateMap<UserId, Guid>().ConvertUsing(src => src.Value);
CreateMap<TagId, Guid>().ConvertUsing(src => src.Value);
CreateMap<FullName, string>().ConvertUsing(src => src.Value);
```

### Feature Mapping Profile

هر Feature Profile از Map پایه استفاده می‌کند و فقط موارد خاص خودش را override می‌کند:

```csharp
// نمونه — InstructorMappingProfile با Custom Resolver
public class InstructorMappingProfile : Profile
{
    public InstructorMappingProfile()
    {
        CreateMap<Instructor, InstructorDto>()
            .ForMember(dest => dest.Avatar,
                opt => opt.MapFrom<InstructorAvatarUrlResolver, string>(src => src.Avatar));
    }
}

// Resolver — برای فیلدهایی که نیاز به سرویس خارجی دارند
public class InstructorAvatarUrlResolver(IFileUrlService fileUrlService)
    : IMemberValueResolver<Instructor, InstructorDto, string, string>
{
    public string Resolve(Instructor source, InstructorDto destination,
        string sourceMember, string destMember, ResolutionContext context)
        => fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, sourceMember);
}
```

> **نکته:** هر Resolver که از سرویس تزریقی استفاده می‌کند باید در `DependencyInjection.cs` ثبت شود.

---

## 🔌 Abstractions — قراردادهای سرویس‌ها

### سرویس‌های پایه

قراردادهایی که پیاده‌سازی‌شان در `Vitastic.Infra` است:

| قرارداد | مسئولیت |
|---|---|
| `IEmailSender` | ارسال ایمیل |
| `IJwtTokenService` | تولید و اعتبارسنجی JWT |
| `IFileStorageService` | ذخیره فایل |
| `IFileUrlService` | تولید URL برای فایل‌های ذخیره‌شده |
| `IPaymentGatewayService` | ارتباط با درگاه پرداخت |
| `ICartIdentityService` | شناسایی سبد خرید کاربر |

### IFile — جایگزین IFormFile

```csharp
// به جای وابستگی مستقیم به IFormFile (که به ASP.NET Core وابسته است)
// از IFile استفاده می‌شود تا Application Layer مستقل بماند
public interface IFile
{
    Stream OpenReadStream();
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }
}
```

> **چرا؟** `IFormFile` به `Microsoft.AspNetCore` وابسته است. استفاده از آن در Application Layer، این لایه را به Web وابسته می‌کند — که نقض اصل معماری است.

### قراردادهای Read Query

برای خواندن داده، به جای استفاده از Repository (که برای Write طراحی شده)، از سرویس‌های Query جداگانه استفاده می‌شود:

| قرارداد | مسئولیت |
|---|---|
| `ICategoryQueryService` | خواندن دسته‌بندی‌ها |
| `ICourseQueryService` | خواندن دوره‌ها |
| `IDiscountQueryService` | خواندن تخفیف‌ها |
| ... | ... |

---

## 🗃️ Data

```csharp
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellation = default);
    Task BeginTransactionAsync(CancellationToken cancellation = default);
    Task CommitAsync(CancellationToken cancellation = default);
    Task RollbackAsync(CancellationToken cancellation = default);
}
```

قرارداد `IUnitOfWork` اینجا تعریف می‌شود — پیاده‌سازی در `Vitastic.Infra` است.

---

## ⚙️ Settings

تنظیمات مربوط به این لایه (مثل تنظیمات Validation یا Behavior) از طریق `DependencyInjection.cs` بارگذاری می‌شوند.

---

## 🔖 ApplicationAssemblyReference

برای ثبت خودکار Handler‌ها و Validator‌های این لایه در MediatR و FluentValidation:

```csharp
// در DependencyInjection.cs لایه‌های دیگر
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(ApplicationAssemblyReference.Assembly));

services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly);
```

</div>