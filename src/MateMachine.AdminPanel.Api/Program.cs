using System.Net;
using System.Net.Http;
using MateMachine.AdminPanel.Api;
using MateMachine.Extensions.AspNet;
using MateMachine.Extensions.AspNet.Models;
using MateMachine.Extensions.AspNet.ServiceMesh;
using MateMachine.Extensions.AspNet.Telemetry;
using MateMachine.Extensions.Data.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddConfig<TelemetryConfig>(builder.Configuration, out var telemetryConfig);
builder.Services.AddAppServices(
    builder.Configuration,
    builder.Environment,
    telemetryConfig
);


//// Get the services we want to have health check monitoring for them
//List<string> serviceNames = new List<string>();
//if (healthCheckConfig.UseAllServices)
//{
//    var httpClient = new HttpClient();
//    var response = httpClient.GetAsync(healthCheckConfig.ServicesUrl).Result;
//    if (response.IsSuccessStatusCode)
//    {
//        var services = JsonConvert.DeserializeObject<List<ServiceDto>>(await response.Content.ReadAsStringAsync());
//        serviceNames = services.Select(s => s.Name).ToList();
//    }
//}
//else
//    serviceNames = healthCheckConfig.Services;

//// Add custom health check for each service
//var bldrHealthCheck = builder.Services.AddHealthChecks();
//foreach (var serviceName in serviceNames)
//{
//    bldrHealthCheck = bldrHealthCheck.AddCustomHealthCheck(
//        serviceName,
//        sp => sp.GetRequiredService<IHealthService>());
//}


//// Add health checks ui
//builder.Services.AddHealthChecksUI()
//    .AddInMemoryStorage();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Host.UseMateTelemetryServices(telemetryConfig);

if (builder.Environment.IsDevelopment())
{
    HttpClient.DefaultProxy = new WebProxy();
}

var app = builder.Build();

app.UseCors();

app.UseMateExceptionHandler(
    EntityFrameworkExceptionHandler.MapKnownExceptions()
);

app.FillInServerAddresses(app.Configuration)
    .UseMateTelemetryServices()
    .UseMateHealthChecks()
    .UseMateServiceDiscovery();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseMateSwagger();
}

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

// Add custom endpoints for health check and its ui
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// var migrator = new DbMigrator(app.Services.GetRequiredService<IServiceScopeFactory>());
// await migrator.StartAsync(app.Lifetime.ApplicationStopping);

await app.RunAsync();

public partial class Program
{
}
