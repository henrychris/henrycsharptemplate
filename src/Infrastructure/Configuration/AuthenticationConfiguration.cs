using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HenryCsharpTemplate.Application.Settings;
using HenryCsharpTemplate.Domain.Models;
using HenryCsharpTemplate.Infrastructure.Persistence;
using HenryCsharpTemplate.Shared;

namespace HenryCsharpTemplate.Infrastructure.Configuration;

internal static class AuthenticationConfiguration
{
    public static void SetupAuthentication(this IServiceCollection services)
    {
        var authSettings = services.BuildServiceProvider().GetService<IOptionsSnapshot<AuthSettings>>()?.Value;

        // JWT Configuration
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authSettings!.Jwt.Issuer,
                    ValidAudience = authSettings!.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings!.Jwt.Secret)),
                    ClockSkew = TimeSpan.Zero,
                };
            });

        services.ConfigureOptions<CustomValidatorJwtBearerOptions>();
    }

    public static void SetupIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
    }

    /// <summary>
    /// Ensures that the application's defined roles exist in the database.
    /// Creates any missing roles.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    public static async Task EnsureRolesCreatedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        var rolesToCreate = Roles.GetAllRoles();
        foreach (var roleName in rolesToCreate)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    logger.LogInformation("Role '{RoleName}' created successfully.", roleName);
                }
                else
                {
                    logger.LogError(
                        "Failed to create role '{RoleName}'. Errors: {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description))
                    );
                }
            }
        }
    }
}

public class CustomValidatorJwtBearerOptions(TimeProvider timeProvider, ILogger<CustomValidatorJwtBearerOptions> logger)
    : IPostConfigureOptions<JwtBearerOptions>
{
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.LifetimeValidator = JwtLifetimeValidator.Create(timeProvider, logger);
    }
}
