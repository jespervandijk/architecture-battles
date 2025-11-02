using Vogen;

namespace AcademicManagement.Domain.Scalars;

[ValueObject<string>]
public partial struct Name
{
    private static Validation Validate(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Validation.Invalid("Name cannot be empty.");
        }

        if (name.Length > 30)
        {
            return Validation.Invalid("Name cannot exceed 30 characters.");
        }

        return Validation.Ok;
    }
}
