using System.Reflection;
using FluentValidation;
using HenryCsharpTemplate.Infrastructure;
using HenryCsharpTemplate.Infrastructure.Configuration;
using HenryCsharpTemplate.Infrastructure.Persistence;
using HenryCsharpTemplate.Infrastructure.Services;
using HenryCsharpTemplate.Shared.Behaviors;
using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Refit.Destructurers;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;

DotEnv.Load(builder.Services, environment);
builder.Configuration.AddEnvironmentVariables().Build();

builder.Services.AddSerilog(
    (services, lc) =>
        lc
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails(
                new DestructuringOptionsBuilder().WithDefaultDestructurers().WithDestructurers(new[] { new ApiExceptionDestructurer() })
            )
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] - {Message:lj}{NewLine}{Exception}"
            )
);

builder.Services.SetupConfigFiles();
builder.Services.SetupDatabase<ApplicationDbContext>();
builder.Services.SetupIdentity();
builder.Services.SetupControllers();
builder.Services.AddOpenApiAndScalar();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.RegisterServices();
builder.Services.SetupJsonOptions();

builder.Services.SetupAuthentication();
builder.Services.SetupCors();
builder.Services.SetupRateLimiting(environment);

// builder.Services.SetupEmail(); // todo: add email
builder.Services.SetupHangfire(environment);

var app = builder.Build();
app.UseCors("DefaultCors");

app.UseSerilogRequestLogging();
await app.ApplyMigrationsAsync<ApplicationDbContext>(environment);

// todo: uncomment later
// await app.EnsureRolesCreatedAsync();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<ISeedService>().SeedDataAsync();

app.RegisterOpenApiAndScalar();
app.RegisterMiddleware();
app.UseRateLimiter();
app.UseHangfireDashboard();
app.Run();

// this is here for integration tests
public partial class Program;
