using Vogen;

namespace AcademicManagement.Domain.Aggregates.Courses;

[ValueObject<int>]
public readonly partial struct StudentCapacity
{
    private static Validation Validate(int capacity)
    {
        return capacity < 1 ? Validation.Invalid("Capacity must be at least 1.") : Validation.Ok;
    }
}
