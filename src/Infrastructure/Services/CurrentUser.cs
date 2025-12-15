using System.Security.Claims;

namespace HenryCsharpTemplate.Infrastructure.Services;

public interface ICurrentUser
{
    string UserId { get; }
    string Username { get; }

    /// <summary>
    /// This isn't tied to whether the user is authenticated or not. It fetches the IP address from the request headers.
    /// </summary>
    string IpAddress { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string UserId =>
        httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new InvalidOperationException("User not authorised");
    public string Username =>
        httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value ?? throw new InvalidOperationException("User not authorised");
    public string IpAddress => Utilities.GetIpAddress(httpContextAccessor.HttpContext?.Request);
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
