using System.ComponentModel;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using Vitastic.Api.Middleware;
using Vitastic.Api.Services;
using Vitastic.App;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Common.Settings;
using Vitastic.Infra;
using Vitastic.Infra.Data;
using Vitastic.Infra.Data.Configurations.Seed;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "https://vitastic.com",
                "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
//-------------------------- Form Options --------------------------//
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 104857600; });

//-------------------------- AutoMapper --------------------------//
builder.Services.AddAutoMapper(config => { config.AddMaps(typeof(Program).Assembly); });

//-------------------------- Exception Handler --------------------------//
builder.Services.AddSingleton<GlobalExceptionHandlerMiddleware>();

//-------------------------- DI Layers --------------------------//
builder.Services.AddHttpContextAccessor();
builder.Services.AddAppServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();
builder.Services.AddScoped<IFileUrlService, FileUrlService>();
builder.Services.AddScoped<ICartIdentityService, CartIdentityService>();

#region ==================== Settings Registration ====================

builder.Services.Configure<ClientSettings>(
    builder.Configuration.GetSection(ClientSettings.SectionName));

// If you want to inject ClientSettings directly (not IOptions<>):
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<ClientSettings>>().Value);

#endregion

//-------------------------- JWT Authentication --------------------------//
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
});
//-------------------------- MVC --------------------------//
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)
    .AddNewtonsoftJson();

//-------------------------- Swagger + Health --------------------------//

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "Vitastic API",
            Version = "v1",
            Description = "Vitastic backend APIs"
        };

        return Task.CompletedTask;
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddHttpClient();

//-------------------------- Serilog --------------------------//
builder.Host.UseSerilog((context, configuration)
    => configuration.ReadFrom.Configuration(context.Configuration));

// ========================= Build =========================
WebApplication app = builder.Build();
// ========================= Seed Data =========================
using (IServiceScope scope = app.Services.CreateScope())
{
    ApplicationWriteDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationWriteDbContext>();
    await context.Database.MigrateAsync();

    var seeder = new DatabaseSeeder(context);
    await seeder.SeedAsync();
}

app.UseSerilogRequestLogging();




app.UseCors("AllowAll");
app.UseRouting();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Vitastic API Docs")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
    // آدرس: /scalar/v1
}

app.UseStaticFiles();

app.UseAuthentication(); // ✅ اول احراز هویت
app.UseAuthorization(); // ✅ بعد مجوزدهی

app.UseMiddleware<GuestSessionMiddleware>();

app.MapControllers();
app.MapHealthChecks("/health");

await app.RunAsync();
