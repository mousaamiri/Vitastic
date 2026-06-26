using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vitastic.App.Common.Behaviors;
using Vitastic.App.Features.Common.Mapping;

namespace Vitastic.App;

public static class DependencyInjection
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(ApplicationAssemblyReference.Assembly);
            config.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });
        //App Services

        //َ Auto mapper
        services.AddAutoMapper(config =>
        {
            config.AddProfile<CourseMappingProfile>();
            config.AddMaps(typeof(BaseMappingProfile).Assembly);
        });
        //Logging Behavior
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        // Register FluentValidation
        services.AddValidatorsFromAssembly(ApplicationAssemblyReference.Assembly, includeInternalTypes: true);
        return services;
    }
}
