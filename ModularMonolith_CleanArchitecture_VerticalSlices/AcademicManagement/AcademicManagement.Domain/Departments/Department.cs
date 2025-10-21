using AcademicManagement.Domain.Professors;

namespace AcademicManagement.Domain.Departments;

public record Department
{
    public DepartmentId Id { get; private set; }

    public string Name { get; private set; }

    public ProfessorId HeadOfDepartment { get; private set; }

    public List<ProfessorId> ProfessorIds { get; private set; }

    private Department(DepartmentId id, string name, ProfessorId headOfDepartment, List<ProfessorId> professorIds)
    {
        Id = id;
        Name = name;
        HeadOfDepartment = headOfDepartment;
        ProfessorIds = professorIds;
    }
    public static Department Create(string name, ProfessorId headOfDepartment)
    {
        return new Department(DepartmentId.Next(), name, headOfDepartment, []);
    }
}

