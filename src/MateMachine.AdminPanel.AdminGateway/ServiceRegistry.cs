using System.Net;
using System.Text.RegularExpressions;
using MateMachine.Extensions.AspNet;
using MateMachine.Extensions.AspNet.Models;
using MateMachine.Extensions.AspNet.Telemetry;
using MateMachine.Identity.ClientSdk;
using MateMachine.Identity.ClientSdk.Config;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace MateMachine.AdminPanel.AdminGateway;

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
            .AddConfig<CorsConfig>(configuration, out var corsConfig)
            .AddConfig<SwaggerConfig>(configuration, out var swaggerConfig)
            .AddConfig<IdentityConfig>(configuration, out var identityConfig)
            .AddConfig<TelemetryConfig>(configuration)
            ;

        var ocelotBuilder = services.AddOcelot(configuration)
            .AddConsul();

        if (webHostEnvironment.IsDevelopment())
        {
            // Action<JwtBearerOptions> options = o =>
            // {
            //     o.Authority = identityConfig.AuthorityUrl;
            //     o.RequireHttpsMetadata = identityConfig.RequireHttps ?? true;
            //     o.TokenValidationParameters = new TokenValidationParameters
            //     {
            //         ValidateAudience = identityConfig.Audience is not null,
            //     };
            //     // etc....
            // };

            // ocelotBuilder.AddAdministration("/admin", options);
            // ocelotBuilder.AddAdministration("/admin", "test");
        }

        return services
                .AddAuth(configuration, webHostEnvironment, identityConfig)
                .AddMateCors(corsConfig)
                .AddOpenTelemetry(configuration, telemetryConfig)
                .AddMateSwagger(swaggerConfig, identityConfig.AuthorityUrl)
            ;
    }

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

    public static IConfigurationBuilder AddMateOcelot(
        this IConfigurationBuilder builder,
        string folder,
        IWebHostEnvironment? env)
    {
        var excludeConfigName = env is null || env.EnvironmentName == null
            ? string.Empty
            : "ocelot." + env.EnvironmentName + ".json";

        var reg = new Regex("^ocelot\\.(.*?)\\.json$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        var list = new DirectoryInfo(folder).EnumerateFiles()
            .Where(fi => reg.IsMatch(fi.Name) && fi.Name != excludeConfigName)
            .ToList();

        var fileConfiguration1 = new FileConfiguration();
        foreach (var fileInfo in list)
        {
            if (list.Count <= 1 || !fileInfo.Name.Equals("ocelot.json", StringComparison.OrdinalIgnoreCase))
            {
                var fileConfiguration2 =
                    JsonConvert.DeserializeObject<FileConfiguration>(File.ReadAllText(fileInfo.FullName));
                if (fileInfo.Name.Equals("ocelot.global.json", StringComparison.OrdinalIgnoreCase))
                    fileConfiguration1.GlobalConfiguration = fileConfiguration2.GlobalConfiguration;
                fileConfiguration1.Aggregates.AddRange(fileConfiguration2.Aggregates);
                fileConfiguration1.Routes.AddRange(fileConfiguration2.Routes);
            }
        }

        File.WriteAllText(Path.Join(env.ContentRootPath, "ocelot.json"),
            JsonConvert.SerializeObject(fileConfiguration1));
        return builder.AddJsonFile("ocelot.json", false, false);
    }

    public static OcelotPipelineConfiguration OcelotConfig =>
        new OcelotPipelineConfiguration()
        {
            PreErrorResponderMiddleware = async (ctx, next) =>
            {
                await next.Invoke();
                var downReq = ctx.Items.DownstreamRequest();
                var downRes = ctx.Items.DownstreamResponse();
                if (downReq is not null &&
                    downRes is null &&
                    ctx.Response.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
        };

    public static IServiceCollection AddOpenTelemetry(
        this IServiceCollection services,
        IConfiguration configuration,
        TelemetryConfig telemetryConfig
    )
    {
        services.AddMateTelemetryServices(configuration, telemetryConfig);
        return services;
    }
}
