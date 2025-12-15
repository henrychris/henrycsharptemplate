using Microsoft.IdentityModel.Tokens;

namespace HenryCsharpTemplate.Shared;

/// <summary>
/// Provides a reusable JWT lifetime validator that uses TimeProvider for testability.
/// This ensures consistent token expiration logic across the application.
/// </summary>
public static class JwtLifetimeValidator
{
    /// <summary>
    /// Creates a lifetime validator delegate that uses the provided TimeProvider.
    /// </summary>
    /// <param name="timeProvider">The time provider to use for getting current UTC time</param>
    /// <param name="logger">Optional logger for validation details</param>
    /// <returns>A LifetimeValidator delegate</returns>
    public static LifetimeValidator Create(TimeProvider timeProvider, ILogger? logger = null)
    {
        return (notBefore, expires, token, parameters) =>
        {
            DateTime utcNow = timeProvider.GetUtcNow().UtcDateTime;

            // Logic based on: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/8.0.1/src/Microsoft.IdentityModel.Tokens/Validation/Validators.Lifetime.cs#L100
            // Thanks to: https://stackoverflow.com/questions/79600635/how-to-setup-jwt-authentication-with-timeprovider-in-net-9-integration-tests

            if (notBefore.HasValue && notBefore.Value > DateTimeUtil.Add(utcNow, parameters.ClockSkew))
            {
                logger?.LogInformation(
                    "Token is not valid before {NotBefore}. Current time (with skew) is {AdjustedTime}.",
                    notBefore.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    DateTimeUtil.Add(utcNow, parameters.ClockSkew).ToString("yyyy-MM-dd HH:mm:ss.fff")
                );
                return false;
            }

            if (expires.HasValue && expires.Value < DateTimeUtil.Add(utcNow, parameters.ClockSkew.Negate()))
            {
                logger?.LogInformation(
                    "Token expired at {Expires}. Current time (with skew) is {AdjustedTime}.",
                    expires.Value.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                    DateTimeUtil.Add(utcNow, parameters.ClockSkew.Negate()).ToString("yyyy-MM-dd HH:mm:ss.fff")
                );
                return false;
            }

            return true;
        };
    }
}
