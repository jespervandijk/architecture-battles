using System;
using Vogen;

namespace AcademicManagement.Domain.Courses;

[ValueObject<int>]
public partial struct GradeWeight
{
    public static Validation Validate(int value)
    {
        return Validation.Ok;
    }
}
