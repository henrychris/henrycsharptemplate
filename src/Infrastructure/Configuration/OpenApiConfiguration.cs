using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace HenryCsharpTemplate.Infrastructure.Configuration;

internal static class OpenApiConfiguration
{
    public static void AddOpenApiAndScalar(this IServiceCollection services)
    {
        // This dictionary will track how many times we've generated an ID for a given type.
        var processedTypeCounts = new ConcurrentDictionary<Type, int>();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();

            // THE HACK:
            // Force unique schema IDs for types that the generator buggily processes multiple times.
            // THE BUG: https://github.com/dotnet/aspnetcore/issues/58213
            options.CreateSchemaReferenceId = (context) =>
            {
                var type = context.Type;
                if (type.FullName is null)
                {
                    return OpenApiOptions.CreateDefaultSchemaReferenceId(context);
                }

                // Increment the count for this type. The new value is returned.
                var count = processedTypeCounts.AddOrUpdate(type, 1, (key, currentCount) => currentCount + 1);

                // Generate the base ID from the full name.
                var baseId = type.FullName.Replace("+", ".");

                // If this is the first time we're seeing this type, use its normal ID.
                // If it's a subsequent time, append a version suffix to make the key unique.
                return count == 1 ? baseId : $"{baseId}.v{count}";
            };
        });
    }

    public static void RegisterOpenApiAndScalar(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.DarkMode = false;
            options.HideModels = true;
            options.WithTitle("HenryCsharpTemplate Api");
        });
    }
}

/// <summary>
/// This adds authentication to the generated scalar UI.
/// https://yogeshhadiya33.medium.com/implement-scalar-in-net-api-91d284479d1d
/// </summary>
/// <param name="authenticationSchemeProvider"></param>
internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();

        if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
        {
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    BearerFormat = "JWT",
                },
            };

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
            {
                operation.Value.Security.Add(
                    new OpenApiSecurityRequirement
                    {
                        [
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme },
                            }
                        ] = Array.Empty<string>(),
                    }
                );
            }
        }
    }
}
