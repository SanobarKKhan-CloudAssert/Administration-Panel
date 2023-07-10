using MateMachine.AdminPanel.AdminGateway;
using MateMachine.Extensions.AspNet;
using MateMachine.Extensions.AspNet.Models;
using MateMachine.Extensions.AspNet.Telemetry;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddMateOcelot("Ocelot", builder.Environment);

builder.Services.AddConfig<TelemetryConfig>(builder.Configuration, out var telemetryConfig);

// Add services to the container.
builder.Services.AddAppServices(
    builder.Configuration,
    builder.Environment,
    telemetryConfig
);


builder.Host.UseMateTelemetryServices(telemetryConfig);

// builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

app.FillInServerAddresses(app.Configuration)
    .UseCors()
    .UseMateTelemetryServices();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseMateSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets();
await app.UseOcelot(ServiceRegistry.OcelotConfig);

await app.RunAsync();

public partial class Program
{
}
