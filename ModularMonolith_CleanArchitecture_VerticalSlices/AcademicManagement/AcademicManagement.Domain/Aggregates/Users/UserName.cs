using Vogen;

namespace AcademicManagement.Domain.Aggregates.Users;

[ValueObject<string>]
public partial struct UserName
{
    private static Validation Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Validation.Invalid("UserName cannot be empty.");
        }

        if (name.Length > 20)
        {
            return Validation.Invalid("UserName cannot exceed 20 characters.");
        }

        return Validation.Ok;
    }
}
