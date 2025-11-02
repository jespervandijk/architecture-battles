using Vogen;

namespace AcademicManagement.Domain.GeneralValueObjects.Users;


[ValueObject<string>]
public partial struct UserRole
{
    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Validation.Invalid("Role cannot be empty.");
        }

        var allowedRoles = new[] { "Admin", "President", "Teacher" };
        if (Array.IndexOf(allowedRoles, value) < 0)
        {
            return Validation.Invalid($"Role '{value}' is not recognized.");
        }

        return Validation.Ok;
    }
    public static UserRole Admin = new("Admin");
    public static UserRole President = new("President");
    public static UserRole Professor = new("Professor");
}
