<div dir="rtl">

# Vitastic.Api 🌐

لایه ارائه — دروازه ورودی تمام درخواست‌های HTTP به سیستم.
این لایه به `Vitastic.App` وابسته است و هیچ منطق کسب‌وکاری مستقیم ندارد — فقط درخواست می‌گیرد، به Handler پاس می‌دهد و پاسخ برمی‌گرداند.

---

## 📋 فهرست مطالب

- [ساختار پوشه‌ها](#️-ساختار-پوشهها)
- [راه‌اندازی و مستندات API](#-راهاندازی-و-مستندات-api)
- [ساختار پاسخ‌ها — ApiResponse](#-ساختار-پاسخها--apiresponse)
- [مدیریت خطا — GlobalExceptionHandler](#-مدیریت-خطا--globalexceptionhandler)
- [Middleware‌ها](#-middlewareها)
- [Extensions](#-extensions)
- [Services](#-services)
- [Wrapper — FormFileAdapter](#-wrapper--formfileadapter)
- [ساختار Features](#-ساختار-features)

---

## 🗂️ ساختار پوشه‌ها

```
Vitastic.Api/
├── Extensions/
│   ├── ClaimsPrincipalExtensions.cs    ← استخراج اطلاعات کاربر از JWT
│   └── ResultExtensions.cs             ← تبدیل Result به ApiResponse
├── Features/                           ← کنترلرها، Request/Response Modelها، AutoMapper Profileها
├── Logs/                               ← فایل‌های لاگ Serilog
├── Middleware/
│   ├── GlobalExceptionHandlerMiddleware.cs
│   └── GuestSessionMiddleware.cs
├── Services/
│   ├── CartIdentityService.cs          ← پیاده‌سازی ICartIdentityService
│   └── FileUrlService.cs              ← پیاده‌سازی IFileUrlService
├── Wrapper/
│   └── FormFileAdapter.cs             ← تبدیل IFormFile به IFile
├── wwwroot/                            ← فایل‌های استاتیک
├── appsettings.json
├── Dockerfile
└── Program.cs
```

---

## 🚀 راه‌اندازی و مستندات API

### پورت‌ها

| Profile | آدرس |
|---|---|
| HTTP | `http://localhost:5275` |
| HTTPS | `https://localhost:7003` |
| Docker | `http://localhost:8080` / `https://localhost:8081` |

### مستندات API — Scalar

در محیط Development، مستندات API از طریق **Scalar** در آدرس زیر قابل دسترس است:

```
https://localhost:7003/scalar/v1
```

> Scalar جایگزین Swagger UI است و تجربه بهتری برای تست API فراهم می‌کند.
> Theme: BluePlanet — نمونه کد به زبان C# با HttpClient.

### ترتیب Pipeline در Program.cs

```
UseSerilogRequestLogging()     ← لاگ تمام درخواست‌ها
UseCors("AllowAll")            ← CORS برای vitastic.com و localhost:3000
UseStaticFiles()
UseAuthentication()            ← احراز هویت JWT
UseAuthorization()             ← بررسی دسترسی
UseMiddleware<GuestSessionMiddleware>()
MapControllers()
MapHealthChecks("/health")     ← Health Check
```

### تنظیمات مهم

```json
// appsettings.json
{
  "Jwt": {
    "Issuer": "...",
    "Audience": "...",
    "SecretKey": "..."
  },
  "ApiSettings": {
    "BaseUrl": "..."
  }
}
```

**محدودیت آپلود فایل:** حداکثر ۱۰۰ مگابایت (`MultipartBodyLengthLimit = 104857600`)

**Seed Data:** در هر بار اجرا، Migration اعمال شده و داده‌های اولیه از طریق `DatabaseSeeder` بارگذاری می‌شوند.

---

## 📦 ساختار پاسخ‌ها — ApiResponse

تمام پاسخ‌های API از یک ساختار یکسان پیروی می‌کنند.
دو نوع پاسخ وجود دارد: `ApiResponse` (بدون داده) و `ApiResponse<T>` (با داده).

```json
// موفق — بدون داده
{
  "isSuccess": true,
  "message": "عملیات موفقیت‌آمیز بود",
  "statusCode": 200,
  "errors": []
}

// موفق — با داده
{
  "isSuccess": true,
  "message": "عملیات موفقیت‌آمیز بود",
  "statusCode": 200,
  "data": { ... },
  "errors": []
}

// ناموفق
{
  "isSuccess": false,
  "message": "تگ با این نام قبلاً ثبت شده است",
  "statusCode": 409,
  "errors": ["Tag.Duplicate:تگ با این نام قبلاً ثبت شده است"]
}
```

### نقشه ErrorType به StatusCode

`ApiResponse` به‌صورت خودکار از `ErrorType` به HTTP Status Code تبدیل می‌کند:

| ErrorType | HTTP Status |
|---|---|
| `Validation` | 400 Bad Request |
| `NotFound` | 404 Not Found |
| `Conflict` | 409 Conflict |
| `Unauthorized` | 401 Unauthorized |
| `Forbidden` | 403 Forbidden |
| `Verification` | 422 Unprocessable Entity |
| `Failure` | 500 Internal Server Error |

### کلاس‌های پایه Feature‌ها

هر کنترلر از `BaseApiController` ارث می‌برد و برای پاسخ‌های صفحه‌بندی‌شده از `PaginatedResponse<T>` استفاده می‌شود:

```csharp
// BaseApiController — مسیر و تنظیمات پایه
[Route("api/[controller]")]
[ApiController]
internal class BaseApiController : ControllerBase { }

// PaginatedResponse — برای لیست‌های صفحه‌بندی‌شده
public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);
```

### تبدیل Result به ApiResponse

`ResultExtensions` این تبدیل را خودکار می‌کند:

```csharp
// Result بدون داده
Result result = await mediator.Send(command, ct);
return result.ToApiResponse("دسته‌بندی با موفقیت ایجاد شد");

// Result<T> با Mapping — رایج‌ترین حالت
Result<Guid> result = await mediator.Send(command, ct);
return result.ToApiResponse(
    id => id,                              // mapper: TSource → TDest
    "دسته‌بندی با موفقیت ایجاد شد");

// Result<T> ناموفق
return result.ToApiResponse<Guid>("خطا در ایجاد دسته‌بندی");
```

### مثال — ساختار کنترلر

```csharp
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class CategoriesController(
    IMediator mediator,
    IMapper mapper,
    ILogger<CategoriesController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> CreateCategory(
        [FromBody] UpsertCategoryRequest request,
        CancellationToken cancellationToken)
    {
        // Request → Command (AutoMapper)
        CreateCategoryCommand command = mapper.Map<CreateCategoryCommand>(request);

        // ارسال به MediatR
        Result<Guid> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.ToApiResponse<Guid>("خطا در ایجاد دسته‌بندی");

        // Result → ApiResponse (با Mapping)
        return result.ToApiResponse(id => id, "دسته‌بندی با موفقیت ایجاد شد");
    }
}
```

---

## 🛡️ مدیریت خطا — GlobalExceptionHandler

`GlobalExceptionHandlerMiddleware` تمام Exception‌های پردازش‌نشده را می‌گیرد و به پاسخ استاندارد **RFC 7807 ProblemDetails** تبدیل می‌کند.

### نقشه Exception به HTTP Status

| Exception | HTTP Status |
|---|---|
| `BusinessRuleViolationException` | 400 Bad Request |
| `FluentValidation.ValidationException` | 422 Unprocessable Entity |
| `EntityNotFoundException` | 404 Not Found |
| `DuplicateEntityException` | 409 Conflict |
| `UniqueConstraintViolatedException` | 409 Conflict |
| `ConcurrencyConflictException` | 409 Conflict |
| `ForbiddenException` | 403 Forbidden |
| `UnauthorizedAccessException` | 401 Unauthorized |
| `TimeoutException` | 408 Request Timeout |
| `RepositoryException` | 500 Internal Server Error |
| `FileStorageException` | 500 Internal Server Error |

### ساختار پاسخ خطا

```json
{
  "type": "https://httpstatuses.io/422",
  "title": "خطای اعتبارسنجی",
  "status": 422,
  "detail": "اطلاعات ارسالی معتبر نیست",
  "instance": "/api/tags",
  "errorCode": "FluentValidation.Error",
  "timestamp": "2025-01-01T00:00:00.000Z",
  "traceId": "0HN...",
  "errors": {
    "name": ["نام تگ نمی‌تواند خالی باشد", "نام تگ حداکثر ۵۰ کاراکتر است"]
  }
}
```

> **نکته:** در محیط Development، فیلدهای `stackTrace` و `innerException` نیز به پاسخ اضافه می‌شوند.
> در Production، پیام‌های خطا کلی‌تر و بدون جزئیات داخلی هستند.

---

## 🔀 Middleware‌ها

### GuestSessionMiddleware

مدیریت Session برای کاربران مهمان (بدون ورود) — برای سبد خرید:

```
درخواست HTTP
      │
      ▼
GuestSessionMiddleware
      │  هدر X-Cart-Session-Id را می‌خواند
      │  و در HttpContext.Items ذخیره می‌کند
      ▼
بقیه Pipeline
```

```http
GET /api/cart
X-Cart-Session-Id: abc-123-guest-session
```

### ترتیب Middleware‌ها در Pipeline

```csharp
app.UseExceptionHandler();          // GlobalExceptionHandler
app.UseMiddleware<GuestSessionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

---

## 🔧 Extensions

### ClaimsPrincipalExtensions

استخراج اطلاعات کاربر از JWT Token:

```csharp
// در Controller
Guid userId = User.GetUserId();           // پرتاب می‌کند اگر احراز هویت نشده باشد
Guid? userId = User.TryGetUserId();       // null برمی‌گرداند اگر یافت نشد
Guid? userId = User.GetCurrentUserId();   // برای کاربر جاری (احراز هویت‌شده یا null)
string? sessionId = HttpContext.GetSessionId(); // Session ID مهمان
```

---

## ⚙️ Services

این سرویس‌ها به `HttpContext` نیاز دارند، پس در لایه API پیاده‌سازی می‌شوند (نه در Infra):

### CartIdentityService

تشخیص هویت کاربر برای سبد خرید — کاربر وارد شده، مهمان، یا ناشناس:

```
GetCartIdentity()
      │
      ├── کاربر احراز هویت شده؟  → CartIdentity.ForUser(userId)
      ├── Session ID دارد؟        → CartIdentity.ForGuest(sessionId)
      └── هیچکدام                → CartIdentity.Anonymous()
```

### FileUrlService

تولید URL کامل برای فایل‌های ذخیره‌شده بر اساس Scheme و Host جاری:

```csharp
// فایل اختصاصی
// → https://api.vitastic.ir/storage/courses/guid-123/Thumbnail/image.jpg

// فایل پیش‌فرض
// → https://api.vitastic.ir/storage/courses/Default/default-course.jpg
```

---

## 🔌 Wrapper — FormFileAdapter

پل ارتباطی بین `IFormFile` (ASP.NET Core) و `IFile` (Application Layer):

```
Controller دریافت می‌کند: IFormFile
        │
        ▼
FormFileAdapter(formFile)      ← تبدیل
        │
        ▼
Command.File = adapter         ← IFile — Application Layer
```

> **چرا؟** Application Layer نباید به `IFormFile` وابسته باشد. `FormFileAdapter` این وابستگی را در لایه API نگه می‌دارد.

---

## 📁 ساختار Features

هر Feature شامل کنترلر، Request/Response Model و AutoMapper Profile است:

```
Features/Categories/
├── CategoriesController.cs
├── Requests/
│   └── UpsertCategoryRequest.cs    ← ورودی از Client
├── Responses/
│   └── CategoryResponse.cs         ← خروجی به Client
└── Profiles/
    └── CategoryProfile.cs          ← AutoMapper: Request→Command و Dto→Response
```

### جریان داده در یک Feature

```
Client Request
      │
      ▼
Request Model          ← ورودی خام از HTTP
      │ AutoMapper
      ▼
Command / Query        ← ارسال به MediatR (لایه App)
      │ MediatR
      ▼
Result<Dto>            ← بازگشت از Handler
      │ ToApiResponse()
      ▼
ApiResponse<Response>  ← پاسخ نهایی به Client
```

> **قانون:** کنترلر هیچ منطقی ندارد — فقط Map می‌کند، Send می‌کند و پاسخ برمی‌گرداند.
> تمام اعتبارسنجی و منطق در لایه App (ValidationBehavior و Handler) انجام می‌شود.

</div>
