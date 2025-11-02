using Vogen;

namespace AcademicManagement.Domain.Aggregates.Courses;

[ValueObject<int>]
public partial struct GradeWeight
{
    public static Validation Validate(int value)
    {
        return Validation.Ok;
    }
}
