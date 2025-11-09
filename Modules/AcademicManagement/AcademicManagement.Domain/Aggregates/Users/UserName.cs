using Vogen;

namespace AcademicManagement.Domain.Aggregates.Users;

[ValueObject<string>]
public readonly partial struct UserName
{
    private static Validation Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Validation.Invalid("UserName cannot be empty.");
        }

        return name.Length > 20 ? Validation.Invalid("UserName cannot exceed 20 characters.") : Validation.Ok;
    }
}
