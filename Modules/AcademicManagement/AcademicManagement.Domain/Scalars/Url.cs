using Vogen;

namespace AcademicManagement.Domain.Scalars;

[ValueObject<string>]
public readonly partial struct Url
{
    private static Validation Validate(string value)
    {
        if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
        {
            return Validation.Invalid($"The value '{value}' is not a valid absolute URL.");
        }

        return Validation.Ok;
    }
}
