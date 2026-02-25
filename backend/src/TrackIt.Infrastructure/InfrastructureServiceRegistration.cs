using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrackIt.Application.Interfaces;
using TrackIt.Domain.Interfaces;
using TrackIt.Infrastructure.Persistence;
using TrackIt.Infrastructure.Persistence.Repositories;
using TrackIt.Infrastructure.Services;

namespace TrackIt.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            ));

        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddMemoryCache();

        services.Configure<ExchangeRateSettings>(configuration.GetSection("ExchangeRates"));
        services.AddHttpClient<IExchangeRateService, ExchangeRateService>(client =>
        {
            var baseUrl = configuration["ExchangeRates:BaseUrl"] ?? "https://openexchangerates.org/api/";
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IPasswordService, PasswordService>();

        return services;
    }
}
