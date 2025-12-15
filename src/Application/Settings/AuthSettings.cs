using System.ComponentModel.DataAnnotations;

namespace HenryCsharpTemplate.Application.Settings;

public class AuthSettings
{
    [Required]
    public JwtSettings Jwt { get; set; } = null!;
}

public class JwtSettings
{
    [Required(AllowEmptyStrings = false)]
    public string Secret { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Issuer { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Audience { get; set; } = null!;
    public int ExpiryInMinutes { get; set; }
    public int RefreshTokenLifetimeInDays { get; set; }
}
