using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Domain.Aggregates.Departments;

public record Department
{
    public DepartmentId Id { get; private set; }

    public string Name { get; private set; }

    public ProfessorId HeadOfDepartment { get; private set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Department() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Department(DepartmentId id, string name, ProfessorId headOfDepartment)
    {
        Id = id;
        Name = name;
        HeadOfDepartment = headOfDepartment;
    }
    public static Department Create(string name, ProfessorId headOfDepartment)
    {
        return new Department(DepartmentId.Next(), name, headOfDepartment);
    }
}

