using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Departments;

public sealed class Department
{
    public DepartmentId Id { get; init; }
    public UniversityId UniversityId { get; set; }
    public Name Name { get; set; }
    public ProfessorId HeadOfDepartment { get; set; }
    public bool IsArchived { get; set; }

    [JsonConstructor]
    private Department(DepartmentId id, UniversityId universityId, Name name, ProfessorId headOfDepartment, bool isArchived)
    {
        Id = id;
        UniversityId = universityId;
        Name = name;
        HeadOfDepartment = headOfDepartment;
        IsArchived = isArchived;
    }
    public static Department Create(UniversityId universityId, Name name, ProfessorId headOfDepartment)
    {
        return new Department(DepartmentId.Next(), universityId, name, headOfDepartment, false);
    }

    public void Update(Name name, ProfessorId headOfDepartment)
    {
        Name = name;
        HeadOfDepartment = headOfDepartment;
    }
}

