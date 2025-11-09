using Vogen;

namespace AcademicManagement.Domain.Aggregates.Users;


[ValueObject<string>]
public partial struct UserRole
{
    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Validation.Invalid("Role cannot be empty.");
        }

        var allowedRoles = new[] { "Admin", "President", "Professor" };
        if (Array.IndexOf(allowedRoles, value) < 0)
        {
            return Validation.Invalid($"Role '{value}' is not recognized.");
        }

        return Validation.Ok;
    }
    public static UserRole Admin => From("Admin");
    public static UserRole President => From("President");
    public static UserRole Professor => From("Professor");
}
