using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Departments;

public sealed class Department
{
    public DepartmentId Id { get; init; }
    public UniversityId UniversityId { get; private set; }
    public Name Name { get; private set; }
    public ProfessorId HeadOfDepartment { get; internal set; }
    public bool IsArchived { get; private set; }

    [JsonConstructor]
    internal Department(DepartmentId id, UniversityId universityId, Name name, ProfessorId headOfDepartment, bool isArchived)
    {
        Id = id;
        UniversityId = universityId;
        Name = name;
        HeadOfDepartment = headOfDepartment;
        IsArchived = isArchived;
    }

    public void UpdateDetails(Name name)
    {
        Name = name;
    }

    public void Archive()
    {
        IsArchived = true;
    }
}

