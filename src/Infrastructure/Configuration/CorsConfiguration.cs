using Microsoft.Extensions.Options;
using HenryCsharpTemplate.Application.Settings;

namespace HenryCsharpTemplate.Infrastructure.Configuration;

internal static class CorsConfiguration
{
    public static void SetupCors(this IServiceCollection services)
    {
        var corsSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<CorsSettings>>()?.Value;
        var allowedUrls = corsSettings?.AllowedUrls?.Split(',').Select(x => x.Trim()).ToArray() ?? [];

        services.AddCors(options =>
            options.AddPolicy("DefaultCors", policy => policy.WithOrigins(allowedUrls).AllowAnyMethod().AllowAnyHeader().AllowCredentials())
        );
    }
}
