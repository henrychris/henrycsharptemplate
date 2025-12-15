using System.Security.Cryptography;
using HenryCsharpTemplate.Application.Settings;

namespace HenryCsharpTemplate;

public static class Utilities
{
    public static string BuildConnectionString(DatabaseSettings databaseSettings)
    {
        return string.IsNullOrEmpty(databaseSettings.ConnectionString)
            ? $"User ID={databaseSettings.UserId}; Password={databaseSettings.Password}; Host={databaseSettings.Host}; Port={databaseSettings.Port}; Database={databaseSettings.DatabaseName}; Pooling=true;"
            : databaseSettings.ConnectionString;
    }

    public static bool IsTestOrDevEnvironment(HttpContext httpContext)
    {
        return httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
            || httpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().EnvironmentName == "Test";
    }

    /// <summary>
    /// Try and get the IP address.
    /// Reference: https://www.reddit.com/r/dotnet/comments/155c4b1/comment/jstr2bg
    /// </summary>
    /// <param name="request">The current HTTP request</param>
    /// <returns></returns>
    public static string GetIpAddress(this HttpRequest? request)
    {
        if (request is null)
        {
            return string.Empty;
        }

        // railway uses X-Real-IP
        var ipAddress = request?.Headers?["X-Real-IP"].ToString();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress;
        }

        ipAddress = request?.Headers?["X-Forwarded-For"].ToString();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            var parts = ipAddress.Split(',');
            if (parts.Length > 0)
            {
                ipAddress = parts[0];
            }

            return ipAddress;
        }

        ipAddress = request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (!string.IsNullOrEmpty(ipAddress))
        {
            return ipAddress;
        }

        return string.Empty;
    }

    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHJKMNPQRSTUVWXYZ123456789";
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        return new string(bytes.Select(b => chars[b % chars.Length]).ToArray());
    }

    // todo: uncomment after adding booking table
    // public static async Task<string> GenerateUniqueBookingCode(ApplicationDbContext context, CancellationToken cancellationToken)
    // {
    //     string code;
    //     do
    //     {
    //         code = GenerateRandomString(BookingConstants.BOOKING_CODE_LENGTH);
    //     } while (await context.Bookings.AnyAsync(b => b.Code == code, cancellationToken));

    //     return code;
    // }
}
