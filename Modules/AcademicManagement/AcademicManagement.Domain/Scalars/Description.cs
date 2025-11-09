using Vogen;

namespace AcademicManagement.Domain.Scalars;

[ValueObject<string>]
public readonly partial struct Description
{
    private static Validation Validate(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return Validation.Invalid("Description cannot be empty.");
        }

        if (description.Length > 500)
        {
            return Validation.Invalid("Description cannot exceed 500 characters.");
        }

        return Validation.Ok;
    }
}
