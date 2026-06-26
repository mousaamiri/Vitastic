using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Middlewares;
var stopwatch = System.Diagnostics.Stopwatch.StartNew();


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddApplicationPart(typeof(Program).Assembly)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    });
Console.WriteLine($"Controllers: {stopwatch.ElapsedMilliseconds}ms");

// Add section
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add HttpClient
builder.Services.AddApiClient(builder.Configuration);
Console.WriteLine($"ApiClient: {stopwatch.ElapsedMilliseconds}ms");

//Mapping profiles
builder.Services.AddAutoMapper(config =>
{
    config.AddMaps(typeof(Program).Assembly);
});
Console.WriteLine($"AutoMapper: {stopwatch.ElapsedMilliseconds}ms");

//Dependency injections

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped< ICourseService,CourseService>();
builder.Services.AddScoped< IInstructorService,InstructorService>();
builder.Services.AddScoped< ICartService,CartService>();
builder.Services.AddScoped< IDiscountService,DiscountService>();
builder.Services.AddScoped< ICheckoutService,CheckoutService>();
builder.Services.AddScoped< IWalletService,WalletService>();
builder.Services.AddScoped< ITransactionService,TransactionService>();
builder.Services.AddScoped<IUserManagerService,UserManagerService>();
builder.Services.AddScoped<IOrderService,OrderService>();
builder.Services.AddScoped<IRoleManagerService,RoleManagerService>();
builder.Services.AddScoped<ICategoryManagerService,CategoryMangerService>();
builder.Services.AddScoped<ITagManagerService,TagManagerService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.Cookie.HttpOnly = true;
        options.SlidingExpiration = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;// محافظت CSRF
    });

WebApplication app = builder.Build();
app.UseStaticFiles();       
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseRouting();
//Use sections
app.UseSession();

app.UseAuthentication();
//After app.UseAuthentication
app.UseMiddleware<AuthGuardMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<TokenRefreshMiddleware>();
// Guest session middleware
app.UseMiddleware<GuestSessionMiddleware>();
app.UseAuthorization();

// For area panels
app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=UserProfile}/{action=Index}/{id?}")
    .WithStaticAssets();

// For main website
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();


