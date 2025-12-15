using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Options;
using HenryCsharpTemplate.Application.Settings;
using HenryCsharpTemplate.Infrastructure.Configuration;

namespace HenryCsharpTemplate.Infrastructure.Configuration;

internal static class HangfireConfiguration
{
    public static void SetupHangfire(this IServiceCollection services, string environment)
    {
        if (environment == "Test")
        {
            return;
        }

        var dbSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<DatabaseSettings>>()?.Value;
        if (dbSettings is null)
        {
            throw new InvalidOperationException("Database settings are not configured");
        }

        var connectionString = Utilities.BuildConnectionString(dbSettings);
        services.AddHangfire(config => config.UsePostgreSqlStorage(post => post.UseNpgsqlConnection(connectionString)));
        services.AddHangfireServer();
    }

    public static void UseHangfireDashboard(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.UseHangfireDashboard("/hangfire");
    }
}
