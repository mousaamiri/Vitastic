<div dir="rtl">

# Vitastic 🎓

پلتفرم فروش دوره‌های آموزشی آنلاین، طراحی‌شده بر پایه معماری **DDD** و **CQRS**، با رابط کاربری مبتنی بر **ASP.NET Core MVC**.

---

## 📋 فهرست مطالب

- [ساختار Solution](#️-ساختار-solution)
- [معماری](#️-معماری)
- [تکنولوژی‌ها](#️-تکنولوژیها)
- [راه‌اندازی](#-راهاندازی)
- [جریان داده](#-جریان-داده)
- [حساب کاربری پیش‌فرض](#-حساب-کاربری-پیشفرض)
- [مستندات بخش‌ها](#-مستندات-بخشها)

---

## 🗂️ ساختار Solution

```
Vitastic/
├── Vitastic.Domain/       # موجودیت‌ها، Value Objectها و قراردادهای دامنه
├── Vitastic.App/          # Use Caseها، Commandها، Queryها و Handlerها (CQRS)
├── Vitastic.Infra/        # پیاده‌سازی Repository، EF Core، سرویس‌های خارجی
├── Vitastic.Api/          # REST API — کنترلرها و Endpointها
├── Vitastic.Web/          # رابط کاربری — ASP.NET Core MVC با Razor Views
└── docker-compose.yml     # راه‌اندازی محیط با Docker
```

---

## 🏗️ معماری

### بخش API — طراحی DDD + CQRS

پروژه از معماری **Domain-Driven Design** در چهار لایه مجزا پیروی می‌کند:

| لایه | پروژه | مسئولیت |
|---|---|---|
| Domain | `Vitastic.Domain` | منطق اصلی کسب‌وکار، موجودیت‌ها، Value Objectها، رویدادهای دامنه |
| Application | `Vitastic.App` | Commandها، Queryها، Handlerها — پیاده‌سازی CQRS |
| Infrastructure | `Vitastic.Infra` | دسترسی به داده، EF Core، پیاده‌سازی Repositoryها |
| Presentation | `Vitastic.Api` | REST API، کنترلرها، Middlewareها |

> **قانون وابستگی:** تمام لایه‌ها به `Vitastic.Domain` وابسته‌اند — Domain به هیچ لایه‌ای وابسته نیست.

### بخش Frontend — ASP.NET Core MVC

پروژه `Vitastic.Web` یک رابط کاربری سمت سرور با **Razor Views** ارائه می‌دهد که از طریق HTTP با API ارتباط برقرار می‌کند.

---

## 🛠️ تکنولوژی‌ها

- **زبان:** C# / .NET
- **API:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **الگوی طراحی:** DDD، CQRS، Clean Architecture
- **Frontend:** ASP.NET Core MVC، Razor Views
- **Container:** Docker، Docker Compose

---

## 🚀 راه‌اندازی

### پیش‌نیازها

- [.NET SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### اجرا با Docker

```bash
git clone https://github.com/your-username/Vitastic.git
cd Vitastic
docker-compose up --build
```

### اجرا بدون Docker

```bash
# اجرای API
cd Vitastic.Api
dotnet run

# اجرای Web در ترمینال جداگانه
cd Vitastic.Web
dotnet run
```

### حساب کاربری پیش‌فرض

در اولین اجرا، `DatabaseSeeder` به‌صورت خودکار یک حساب ادمین می‌سازد:

| فیلد | مقدار |
|---|---|
| ایمیل | `admin@vitastic.com` |
| نام کاربری | `admin` |
| رمز عبور | `1234Ma@` |

> ⚠️ بعد از اولین ورود، رمز عبور را تغییر دهید.

---

## 📁 جریان داده

```
┌─────────────────────────────────────────────────────────┐
│                      Vitastic.Web                       │
│                  (ASP.NET Core MVC)                     │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTP Request
                           ▼
┌─────────────────────────────────────────────────────────┐
│                      Vitastic.Api                       │
│               (Controllers / Middleware)                │
└──────────────────────────┬──────────────────────────────┘
                           │ Command / Query
                           ▼
┌─────────────────────────────────────────────────────────┐
│                      Vitastic.App                       │
│              (CQRS Handlers / Use Cases)                │
└────────────┬─────────────────────────────┬──────────────┘
             │ Domain Logic                │ Persistence
             ▼                             ▼
┌────────────────────────┐   ┌─────────────────────────────┐
│    Vitastic.Domain     │◄──│       Vitastic.Infra         │
│  ╔══════════════════╗  │   │  (EF Core / Repositories)   │
│  ║  ★ مرکز اصلی ★  ║  │   └─────────────────────────────┘
│  ╚══════════════════╝  │
│  Entities / Events /   │
│  Value Objects /       │
│  Contracts             │
└────────────────────────┘

 ► جهت وابستگی: همه لایه‌ها به Domain وابسته‌اند
 ◄ جهت فراخوانی: Infra قراردادهای Domain را پیاده‌سازی می‌کند
```

---

## 📚 مستندات بخش‌ها

برای جزئیات معماری و نکات توسعه هر بخش، README مربوطه را مطالعه کنید:

| بخش | توضیح |
|---|---|
| [Vitastic.Domain](./Vitastic.Domain/README.md) | سلسله‌مراتب موجودیت‌ها، Domain Eventها، قراردادها |
| [Vitastic.App](./Vitastic.App/README.md) | CQRS، Command/Query، Handler pattern |
| [Vitastic.Infra](./Vitastic.Infra/README.md) | EF Core، Repository، Migration |
| [Vitastic.Api](./Vitastic.Api/README.md) | Endpointها، Middleware، مدیریت خطا |
| [Vitastic.Web](./Vitastic.Web/README.md) | MVC، Razor Views، ارتباط با API |

---

## 📄 لایسنس

این پروژه تحت لایسنس [MIT](LICENSE) منتشر شده است.

</div>