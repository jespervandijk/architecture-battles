using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Departments;

public sealed class Department
{
    public DepartmentId Id { get; init; }
    public Name Name { get; set; }
    public ProfessorId HeadOfDepartment { get; set; }
    public List<ProfessorId> ProfessorIds { get; set; }

    [JsonConstructor]
    private Department(DepartmentId id, Name name, ProfessorId headOfDepartment, List<ProfessorId> professorIds)
    {
        Id = id;
        Name = name;
        HeadOfDepartment = headOfDepartment;
        ProfessorIds = professorIds;
    }
    public static Department Create(Name name, ProfessorId headOfDepartment)
    {
        return new Department(DepartmentId.Next(), name, headOfDepartment, []);
    }
}

