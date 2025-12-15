namespace HenryCsharpTemplate.Shared;

/// <summary>
/// Defines constants for application role names.
/// </summary>
public static class Roles
{
    public const string SUPER_ADMIN = "SuperAdmin";
    public const string VENUE_MANAGER = "VenueManager";
    public const string USER = "User";
    public const string STAFF = "Staff";

    /// <summary>
    /// Gets a list of all defined roles.
    /// </summary>
    /// <returns>A list containing all role names.</returns>
    public static List<string> GetAllRoles()
    {
        return [SUPER_ADMIN, VENUE_MANAGER, USER, STAFF];
    }
}

public static class BookingConstants
{
    public const int BOOKING_CODE_LENGTH = 8;
}
