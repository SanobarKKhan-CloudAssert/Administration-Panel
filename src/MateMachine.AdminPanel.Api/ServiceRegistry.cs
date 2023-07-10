using System;
using MassTransit;
using MateMachine.Extensions.AspNet;
using MateMachine.Extensions.AspNet.MessageBus;
using MateMachine.Extensions.AspNet.MessageBus.Config;
using MateMachine.Extensions.AspNet.Models;
using MateMachine.Extensions.AspNet.ServiceMesh;
using MateMachine.Extensions.AspNet.Telemetry;
using MateMachine.Identity.ClientSdk;
using MateMachine.Identity.ClientSdk.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MateMachine.AdminPanel.Api;

public static class ServiceRegistry
{
    public static IServiceCollection AddAppServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment,
        TelemetryConfig telemetryConfig
    )
    {
        services
            // .AddConfig<RedisConfig>(configuration, out var redisConfig)
            .AddConfig<MessageBusConfig>(configuration, out var messageBusConfig)
            // .AddConfig<AppConfig>(configuration, out var appConfig)
            .AddConfig<IdentityConfig>(configuration, out var identityConfig)
            .AddConfig<SwaggerConfig>(configuration, out var swaggerConfig)
            // .AddConfig<DbOptions>(configuration, out var dbOptions)
            .AddConfig<ServiceDiscoveryConfig>(configuration, out var serviceDiscoveryConfig)
            .AddConfig<TelemetryConfig>(configuration);

        services.AddSingleton(() => DateTime.UtcNow);

        return services
            .AddServices(configuration)
            .AddMateHealthChecks(configuration)
            .AddMateServiceDiscovery(serviceDiscoveryConfig)
            // .AddDataContext(dbOptions)
            // .AddRedis(configuration, webHostEnvironment, redisConfig)
            .AddMessageQueue(configuration, webHostEnvironment, messageBusConfig)
            .AddAuth(configuration, webHostEnvironment, identityConfig)
            .AddOpenTelemetry(configuration, webHostEnvironment, telemetryConfig)
            .AddMateSwagger(swaggerConfig, identityConfig.AuthorityUrl);
    }

    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }

    private static IServiceCollection AddMessageQueue(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment,
        MessageBusConfig config
    )
    {
        services.AddMateMessageBus(config, cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            // cfg.AddConsumersFromNamespaceContaining<StartChatMessageConsumer>();
        });

        return services;
    }

    public static void ConfigMessageQueue(
        this IBusRegistrationConfigurator c
    )
    {
    }

    // private static IServiceCollection AddRedis(
    //     this IServiceCollection services,
    //     IConfiguration configuration,
    //     IWebHostEnvironment webHostEnvironment,
    //     RedisConfig config
    // )
    // {
    //     // configure redis
    //     var redisOptions = ConfigurationOptions.Parse($"{config.Hostname}:{config.Port},allowAdmin=true");
    //     services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect(redisOptions));
    //
    //     return services;
    // }

    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment,
        IdentityConfig identityConfig
    )
    {
        services.AddMateIdentity(identityConfig);

        return services;
    }


    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment webHostEnvironment,
        TelemetryConfig telemetryConfig
    )
    {
        services.AddMateTelemetryServices(configuration, telemetryConfig);

        return services;
    }

    // public static IServiceCollection AddDataContext(
    //     this IServiceCollection services,
    //     DbOptions dbOptions
    // )
    // {
    //     services.AddDbContext<DataContext>(opt => opt.ConfigAppDb(dbOptions));
    //
    //     services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DataContext>());
    //
    //     return services;
    // }
    //
    // public static void ConfigAppDb(
    //     this DbContextOptionsBuilder builder,
    //     DbOptions dbOptions
    // )
    // {
    //     ArgumentNullException.ThrowIfNull(builder);
    //     ArgumentNullException.ThrowIfNull(dbOptions);
    //
    //     builder.UseSqlServer(
    //         dbOptions.ToString(),
    //         opt => opt.ExecutionStrategy(
    //             x => new SqlServerRetryingExecutionStrategy(x.CurrentContext.Context)
    //         )
    //     );
    // }
}
