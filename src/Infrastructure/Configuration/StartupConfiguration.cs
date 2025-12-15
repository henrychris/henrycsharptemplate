using System.Text.Json;
using System.Text.Json.Serialization;
using HenryUtils.Middleware;
using Microsoft.AspNetCore.Mvc;

namespace HenryCsharpTemplate.Infrastructure.Configuration;

internal static class StartupConfiguration
{
    public static void SetupControllers(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddRouting(options => options.LowercaseUrls = true);

        // validation is performed using FluentValidation and a specific response body is used.
        // the custom filter is CustomValidationFilter.cs
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
    }

    /// <summary>
    /// Configure the JSON options for the application.
    /// </summary>
    /// <param name="services"></param>
    public static void SetupJsonOptions(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(jsonOptions =>
        {
            jsonOptions.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonOptions.JsonSerializerOptions.WriteIndented = false;
            jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonOptions.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            jsonOptions.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
    }

    public static void RegisterMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
