using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TrackIt.Application.Commands.CreateSubscription;
using TrackIt.Application.Common.Behaviors;
using TrackIt.Application.Mappings;

namespace TrackIt.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateSubscriptionCommand).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(CreateSubscriptionCommandValidator).Assembly);

        return services;
    }
}
