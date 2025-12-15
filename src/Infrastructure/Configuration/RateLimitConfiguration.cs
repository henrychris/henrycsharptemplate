using System.Threading.RateLimiting;
using HenryCsharpTemplate.Infrastructure.Services;

namespace HenryCsharpTemplate.Infrastructure.Configuration;

internal static class RateLimitConfiguration
{
    internal class RateLimitResponse
    {
        public required string Error { get; set; }
        public required string Message { get; set; }
        public double? RetryAfter { get; set; }
    }

    public static void SetupRateLimiting(this IServiceCollection services, string environment)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RateLimitResponse>>();
                var currentUser = context.HttpContext.RequestServices.GetRequiredService<ICurrentUser>();

                // Get rate limit details
                var requestPath = context.HttpContext.Request.Path;
                var identifier = currentUser.IsAuthenticated ? $"User:{currentUser.UserId}" : $"IP:{currentUser.IpAddress}";

                logger.LogWarning("Rate limit exceeded. Path: {Path}, Identifier: {Identifier}", requestPath, identifier);
                context.HttpContext.Response.ContentType = "application/json";

                var response = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? new RateLimitResponse
                    {
                        Error = "Too many requests",
                        Message = $"Please try again after {retryAfter.TotalMinutes} minute(s)",
                        RetryAfter = retryAfter.TotalMinutes,
                    }
                    : new RateLimitResponse
                    {
                        Error = "Too many requests",
                        Message = "Please try again later",
                        RetryAfter = null,
                    };

                context.HttpContext.Response.Headers.Append("Retry-After", ((int)retryAfter.TotalSeconds).ToString());

                logger.LogDebug("Rate limit response details. RetryAfter: {RetryAfter}, Message: {Message}", response.RetryAfter, response.Message);

                await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };

            // Global rate limits with user/IP partitioning
            if (environment != Environments.Development && environment != "Test")
            {
                options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                    // 100 requests per minute
                    PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
                        var partitionKey = currentUser.IsAuthenticated ? currentUser.UserId : currentUser.IpAddress;

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 100,
                                Window = TimeSpan.FromMinutes(1),
                                AutoReplenishment = true,
                            }
                        );
                    }),
                    // 1000 requests per hour
                    PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var currentUser = context.RequestServices.GetRequiredService<ICurrentUser>();
                        var partitionKey = currentUser.IsAuthenticated ? currentUser.UserId : currentUser.IpAddress;

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 1000,
                                Window = TimeSpan.FromHours(1),
                                AutoReplenishment = true,
                            }
                        );
                    })
                );
            }
        });
    }
}
