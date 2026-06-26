<div dir="rtl">

# Vitastic.Web 🖥️

لایه رابط کاربری — یک ASP.NET Core MVC با Razor Views که از طریق HTTP با `Vitastic.Api` ارتباط برقرار می‌کند.
این لایه هیچ دسترسی مستقیمی به دیتابیس یا Domain ندارد — تمام داده‌ها از API دریافت می‌شوند.

> ⚠️ **در دست توسعه:** این بخش آخرین لایه‌ای است که به پروژه اضافه شده و هنوز کامل نیست.
> زیرساخت اصلی (ApiClient، Middlewareها، احراز هویت) پیاده‌سازی شده،
> اما برخی Viewها و Featureها در حال تکمیل هستند.

---

## 📋 فهرست مطالب

- [ساختار پوشه‌ها](#️-ساختار-پوشهها)
- [ApiClient — هسته ارتباط با API](#-apiclient--هسته-ارتباط-با-api)
- [Services — لایه بالای ApiClient](#-services--لایه-بالای-apiclient)
- [Middlewareها](#-middlewareها)
- [Areas](#-areas)

---

## 🗂️ ساختار پوشه‌ها

```
Vitastic.Web/
├── Areas/
│   ├── AdminPanel/             ← پنل مدیریت
│   └── UserPanel/              ← پنل کاربری
├── Controllers/                ← کنترلرهای عمومی
├── Helper/
├── Infrastructure/
│   ├── ApiClient/              ← هسته HTTP Client
│   │   ├── IApiClient.cs
│   │   ├── ApiClient.cs
│   │   ├── ApiClientServiceExtension.cs
│   │   ├── ApiResponse.cs
│   │   ├── PaginatedApiResponse.cs
│   │   └── PaginatedData.cs
│   └── Services/               ← سرویس‌های اختصاصی هر Entity
├── Middlewares/
├── Models/                     ← ViewModel‌ها و DTO‌ها
├── ViewComponents/
├── Views/                      ← Razor Views
└── wwwroot/                    ← فایل‌های استاتیک (CSS، JS، تصاویر)
```

---

## 🔌 ApiClient — هسته ارتباط با API

`IApiClient` یک HTTP Client عمومی است که تمام متدهای HTTP را پوشش می‌دهد
و پاسخ‌ها را به‌صورت خودکار به `ApiResponse<T>` تبدیل می‌کند.

### قراردادها

```csharp
public interface IApiClient
{
    // GET
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, CancellationToken ct = default);
    Task<ApiResponse<T>> GetAsync<T>(string endpoint, object? queryParams, CancellationToken ct = default);
    Task<PaginatedApiResponse<T>> GetPaginatedAsync<T>(string endpoint, object? queryParams = null, CancellationToken ct = default);

    // POST / PUT / PATCH / DELETE
    Task<ApiResponse> PostAsync(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse<T>> PostAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse> PutAsync(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse<T>> PutAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse> PatchAsync(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse<T>> PatchAsync<T>(string endpoint, object? body = null, CancellationToken ct = default);
    Task<ApiResponse> DeleteAsync(string endpoint, CancellationToken ct = default);
    Task<ApiResponse<T>> DeleteAsync<T>(string endpoint, CancellationToken ct = default);

    // Multipart — آپلود فایل
    Task<ApiResponse> PostMultipartAsync(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse<T>> PostMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse> PutMultipartAsync(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse<T>> PutMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse> PatchMultipartAsync(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);
    Task<ApiResponse<T>> PatchMultipartAsync<T>(string endpoint, MultipartFormDataContent content, CancellationToken ct = default);

    // Download
    Task<byte[]?> DownloadAsync(string endpoint, CancellationToken ct = default);

    // Helper برای ساخت Multipart Content
    MultipartFormDataContent BuildMultipartContent(
        Dictionary<string, string>? formFields = null,
        Dictionary<string, (Stream Stream, string FileName)>? files = null);
}
```

### ساختار پاسخ‌ها

```csharp
// پاسخ ساده
public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }
    public List<string> Errors { get; set; }
}

// پاسخ صفحه‌بندی‌شده
public class PaginatedApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public PaginatedData<T>? Data { get; set; }
}

public class PaginatedData<T>
{
    public IReadOnlyList<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

### ثبت در DI

```csharp
// تنظیمات در appsettings.json
"ApiSettings": {
  "BaseUrl": "https://api.vitastic.ir/",
  "TimeoutSeconds": 30
}

// ثبت در Program.cs
builder.Services.AddApiClient(builder.Configuration);
```

---

## ⚙️ Services — لایه بالای ApiClient

به جای استفاده مستقیم از `IApiClient` در Controller‌ها، برای هر Entity یک Service اختصاصی تعریف می‌شود که Endpointها و DTO‌های مربوط را캡سوله می‌کند.

### الگو

```
IApiClient                        ← HTTP Client عمومی
      │
      ▼
IAuthService / ICategoryService / ...   ← سرویس اختصاصی هر Entity
      │
      ▼
Controller                        ← فقط با سرویس کار می‌کند
```

### مثال — AuthService

```csharp
public class AuthService(IApiClient apiClient) : IAuthService
{
    public Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto model)
        => apiClient.PostAsync<LoginResponseDto>("Users/login", model);

    public Task<ApiResponse<Guid>> RegisterAsync(RegisterDto model)
        => apiClient.PostAsync<Guid>("Users/register", model);

    public Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(string refreshToken)
        => apiClient.PostAsync<LoginResponseDto>("Users/refresh", new { refreshToken });

    public Task<ApiResponse> LogoutAsync()
        => apiClient.PostAsync("Users/logout");

    // ...سایر متدها
}
```

### مثال — استفاده در Controller

```csharp
public class AccountController(IAuthService authService) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var response = await authService.LoginAsync(new LoginDto(model.Email, model.Password));

        if (!response.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, response.Message);
            return View(model);
        }

        // ذخیره Token در Cookie و ریدایرکت
        return RedirectToAction("Index", "Home");
    }
}
```

> **قانون:** Controller هرگز مستقیم از `IApiClient` استفاده نمی‌کند — همیشه از سرویس اختصاصی.
> این کار Endpoint‌ها را در یک جا نگه می‌دارد و Controller را تمیز می‌کند.

---

## 🔀 Middlewareها

```
Request
   │
   ▼
AuthGuardMiddleware        ← محافظت از مسیرهای نیازمند احراز هویت
   │
   ▼
TokenRefreshMiddleware     ← تمدید خودکار JWT قبل از انقضا (5 دقیقه مانده)
   │
   ▼
GuestSessionMiddleware     ← ساخت و مدیریت Session ID برای کاربر مهمان
   │
   ▼
ErrorHandlingMiddleware    ← مدیریت Exception‌های پردازش‌نشده
   │
   ▼
Controller / Action
```

### AuthGuardMiddleware

مسیرهای `/user-panel` و `/admin` را محافظت می‌کند — کاربر احراز هویت نشده به صفحه Login ریدایرکت می‌شود:

```
/user-panel/** یا /admin/**
        │
        ├── احراز هویت شده؟  → ادامه
        └── نشده؟            → Redirect به /Account/Login?returnUrl=...
```

### TokenRefreshMiddleware

اگر کاربر احراز هویت شده باشد و Token کمتر از ۵ دقیقه به انقضا داشته باشد، Token به‌صورت خودکار تمدید می‌شود:

```
Token تا ۵ دقیقه دیگر منقضی می‌شود؟
        │
        ├── بله → RefreshTokenAsync → SignInAsync با Token جدید
        └── خطا یا Refresh Token نامعتبر → SignOutAsync (Logout خودکار)
```

### GuestSessionMiddleware

برای کاربران مهمان (بدون Login) یک Session ID یکتا می‌سازد و در Cookie ذخیره می‌کند تا سبد خرید بین درخواست‌ها حفظ شود:

```
کاربر احراز هویت نشده؟
        │
        ├── Session ID در Cookie یا Session دارد؟  → استفاده از همان
        └── ندارد؟  → ساخت Guid جدید + ذخیره در Cookie (30 روز)
```

> **نکته:** Session ID از Cookie خوانده شده و در `context.Session` هم ذخیره می‌شود
> تا در همان Request بدون نیاز به Cookie مجدد قابل دسترس باشد.

### ErrorHandlingMiddleware

Exception‌های پردازش‌نشده را می‌گیرد و کاربر را به صفحه خطای مناسب هدایت می‌کند:

| Exception | HTTP Status | ریدایرکت |
|---|---|---|
| `UnauthorizedAccessException` | 401 | `/Error?code=401` |
| `KeyNotFoundException` | 404 | `/Error?code=404` |
| سایر خطاها | 500 | `/Error?code=500` |

---

## 🗂️ Areas

| Area | مسئولیت |
|---|---|
| `AdminPanel` | مدیریت دوره‌ها، کاربران، دسته‌بندی‌ها، تخفیف‌ها و ... |
| `UserPanel` | پروفایل کاربر، دوره‌های خریداری‌شده، کیف پول، سفارش‌ها |

</div>
