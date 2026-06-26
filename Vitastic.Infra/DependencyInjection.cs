using AutoMapper;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Data;
using Vitastic.App.Features.Common.Mapping;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.BackgroundJobs;
using Vitastic.Infra.Data;
using Vitastic.Infra.Interceptor;
using Vitastic.Infra.Repositories;
using Vitastic.Infra.Services.Base;
using Vitastic.Infra.Services.Queries;

namespace Vitastic.Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {


        var connectionString = configuration.GetConnectionString("DefaultConnectionString");
        // Add other repositories here
        services.AddSingleton<ConvertDomainEventToOutboxMessageInterceptor>();
        services.AddDbContext<ApplicationWriteDbContext>((sq, options) =>
            {
                ConvertDomainEventToOutboxMessageInterceptor? interceptor =
                    sq.GetService<ConvertDomainEventToOutboxMessageInterceptor>();
                options.UseNpgsql(connectionString)
                    .AddInterceptors(interceptor)
                    .UseExceptionProcessor()
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }
        );

        // Add MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });
        // Add Quartz
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            configure.AddJob<ProcessOutboxMessagesJob>(jobKey, c => { })
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .StartNow()
                    .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(10).RepeatForever()));
        });

        //JWT Configuration
        services.Configure<JwtSettings>(settings =>
        {
            IConfigurationSection jwtSection = configuration.GetSection("Jwt");
            settings.Issuer = jwtSection["Issuer"] ?? string.Empty;
            settings.Audience = jwtSection["Audience"] ?? string.Empty;
            settings.SecretKey = jwtSection["SecretKey"] ?? string.Empty;

            if (int.TryParse(jwtSection["AccessTokenExpirationMinutes"], out var accessMinutes))
                settings.AccessTokenExpirationMinutes = accessMinutes;

            if (int.TryParse(jwtSection["RefreshTokenExpirationDays"], out var refreshDays))
                settings.RefreshTokenExpirationDays = refreshDays;
        });

        services.AddSingleton<IJwTokenService, JwTokenService>();
        // Api Authentication with JWT & Bearer
        //Quartz Hosted Service
        services.AddQuartzHostedService(q => q.WaitForJobsToComplete=true);
        //Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        //Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IInstructorRepository,InstructorRepository>();
        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IUserRoleRepository,UserRoleRepository>();
        services.AddScoped<IInstructorRatingRepository,InstructorRatingRepository>();
        services.AddScoped<ICourseRatingRepository,CourseRatingRepository>();
        services.AddScoped<ICourseTagRepository, CourseTagRepository>();
        services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
        services.AddScoped<ICartRepository,CartRepository>();
        services.AddScoped<ICartItemRepository,CartItemRepository>();
        //Value resolver for imapper
        services.AddScoped<InstructorAvatarUrlResolver>();
        services.AddScoped<CartItemCourseImageUrlResolver>();
        services.AddScoped<OrderItemDtoThumbnailUrlResolver>();
        services.AddScoped<UserAvatarUrlResolver>();
        services.AddScoped<CourseInstructorAvatarUrlResolver>();
        services.AddScoped<CourseVideoUrlResolver>();
        services.AddScoped<CourseThumbnailUrlResolver>();
        services.AddScoped<CourseImageUrlResolver>();
        //Read Services
        services.AddScoped<ICategoryQueryService>(sp =>
            new CategoryQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<CategoryQueryService>>()));
        services.AddScoped<ITagQueryService>(sp =>
            new TagQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<TagQueryService>>()));
        services.AddScoped<IInstructorQueryService>(sp =>
            new InstructorQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<InstructorQueryService>>()));
        services.AddScoped<IOrderQueryService>(sp =>
            new OrderQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<OrderQueryService>>()));

        services.AddScoped<IPaymentTransactionQuery>(sp => new PaymentTransactionQuery(
            connectionString,
            sp.GetRequiredService<ApplicationWriteDbContext>(),
            sp.GetRequiredService<IMapper>(),
            sp.GetRequiredService<ILogger<PaymentTransactionQuery>>())
        );
        services.AddScoped<ICourseQueryService>(sp =>
            new CourseQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<IFileUrlService>(),
                sp.GetRequiredService<ILogger<CourseQueryService>>(),
                sp.GetRequiredService<IFileUrlService>()
        ));
        services.AddScoped<IRoleQueryService>(sp =>
            new RoleQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<RoleQueryService>>()));

        services.AddScoped<IWalletQueryService>(sp =>
            new WalletQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<WalletQueryService>>()));

        services.AddScoped<IUserQueryService>(sp =>
            new UserQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<IFileUrlService>(),
                sp.GetRequiredService<ILogger<UserQueryService>>()));

        services.AddScoped<IDiscountQueryService>(sp =>
            new DiscountQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<DiscountQueryService>>()));

        services.AddScoped<IPermissionQueryService>(sp =>
            new PermissionQueryService(
                connectionString,
                sp.GetRequiredService<ApplicationWriteDbContext>(),
                sp.GetRequiredService<IMapper>(),
                sp.GetRequiredService<ILogger<PermissionQueryService>>()));
        //Services
        services.AddScoped<IFileStorageService, InternalStorageService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();


        return services;
    }
}
