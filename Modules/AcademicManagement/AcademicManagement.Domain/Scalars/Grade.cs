using Vogen;

namespace AcademicManagement.Domain.Scalars;

[ValueObject<double>]
public readonly partial struct Grade
{
    private static Validation Validate(double value)
    {
        if (value is < 0.0 or > 10.0)
        {
            return Validation.Invalid($"Grade must be between 0.0 and 10.0, but was {value}.");
        }

        return Validation.Ok;
    }
}
