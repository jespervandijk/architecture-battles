using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Domain.Aggregates.Departments;

public record Department
{
    public DepartmentId Id { get; init; }

    public string Name { get; internal set; }

    public ProfessorId HeadOfDepartment { get; internal set; }

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

